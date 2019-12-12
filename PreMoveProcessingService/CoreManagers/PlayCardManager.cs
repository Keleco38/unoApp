using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Common.Enums;
using Common.Extensions;
using EntityObjects;
using EntityObjects.Cards.Abstraction;
using GameProcessingService.CardEffectProcessors.AutomaticallyTriggered;
using GameProcessingService.CardEffectProcessors.Played;
using GameProcessingService.CoreManagers;
using GameProcessingService.Models;

namespace PreMoveProcessingService.CoreManagers
{
    public class PlayCardManager : IPlayCardManager
    {
        private readonly IEnumerable<IPlayedCardEffectProcessor> _playableCardEffectProcessors;
        private readonly IEnumerable<IAutomaticallyTriggeredCardEffectProcessor> _automaticallyTriggeredCardEffectProcessors;
        private readonly IGameManager _gameManager;

        public PlayCardManager(IEnumerable<IPlayedCardEffectProcessor> playableCardEffectProcessors, IEnumerable<IAutomaticallyTriggeredCardEffectProcessor> automaticallyTriggeredCardEffectProcessors, IGameManager gameManager)
        {
            _playableCardEffectProcessors = playableCardEffectProcessors;
            _automaticallyTriggeredCardEffectProcessors = automaticallyTriggeredCardEffectProcessors;
            _gameManager = gameManager;
        }

        public MoveResult PlayCard(Game game, Player playerPlayed, string cardPlayedId, CardColor targetedCardColor, string playerTargetedId, string cardToDigId, List<int> duelNumbers,
            List<string> charityCardsIds, int blackjackNumber, List<int> numbersToDiscard, string cardPromisedToDiscardId, string oddOrEvenGuess, CardValue targetedCardValue,
            bool activateSpecialCardEffect)
        {
            var cardPlayed = playerPlayed.Cards.FirstOrDefault(x => x.Id == cardPlayedId);
            if (cardPlayed == null)
                return null;

            //check if valid move
            if (!IsValidMove(game, playerPlayed, cardPlayed))
                return null;


            playerPlayed.Cards.Remove(cardPlayed);
            game.DiscardedPile.Add(cardPlayed);

            game.Players.ForEach(x => x.Cards.Shuffle());

            var playerTargeted = string.IsNullOrEmpty(playerTargetedId) ? _gameManager.GetNextPlayer(game, playerPlayed, game.Players) : game.Players.First(x => x.Id == playerTargetedId);
            var colorForLastCard = targetedCardColor == 0 ? cardPlayed.Color : targetedCardColor;

            if (colorForLastCard == CardColor.Wild)
            {
                colorForLastCard = game.LastCardPlayed.Color;
            }

            var previousLastCardPlayed = game.LastCardPlayed;

            game.LastCardPlayed = new LastCardPlayed(colorForLastCard, cardPlayed.Value, cardPlayed.ImageUrl, playerPlayed.User.Name, cardPlayed.Color == CardColor.Wild, cardPlayed);

            var cardToDig = string.IsNullOrEmpty(cardToDigId) ? null : game.DiscardedPile.First(x => x.Id == cardToDigId);
            var cardPromisedToDiscard = string.IsNullOrEmpty(cardPromisedToDiscardId) ? null : playerPlayed.Cards.First(x => x.Id == cardPromisedToDiscardId);
            var charityCards = charityCardsIds != null ? playerPlayed.Cards.Where(x => charityCardsIds.Contains(x.Id)).ToList() : null;

            var messagesToLog = new List<string>();

            //pre play check promise keeper
            var automaticallyTriggeredResultPromiseKeeper = _automaticallyTriggeredCardEffectProcessors.First(x => x.CardAffected == CardValue.PromiseKeeper).ProcessCardEffect(game, string.Empty, new AutomaticallyTriggeredParams() { PromiseKeeperParams = new AutomaticallyTriggeredPromiseKeeperParams(playerPlayed, cardPlayed) });
            if (!string.IsNullOrEmpty(automaticallyTriggeredResultPromiseKeeper.MessageToLog))
                messagesToLog.Add(automaticallyTriggeredResultPromiseKeeper.MessageToLog);

            //play card effect
            var moveParams = new MoveParams(playerPlayed, cardPlayed, playerTargeted, colorForLastCard, cardToDig, duelNumbers, charityCards, blackjackNumber, numbersToDiscard, cardPromisedToDiscard, oddOrEvenGuess, previousLastCardPlayed, targetedCardValue, activateSpecialCardEffect);
            MoveResult moveResult;
            if (game.SilenceTurnsRemaining > 0 && cardPlayed.Color == CardColor.Wild && cardPlayed.Value != CardValue.Silence)
            {
                moveResult = new MoveResult(new List<string>() { $"Game is silenced for {game.SilenceTurnsRemaining} moves. {cardPlayed.Value.ToString()} had no effect." });
            }
            else
            {
                moveResult = _playableCardEffectProcessors.First(x => x.CardAffected == cardPlayed.Value).ProcessCardEffect(game, moveParams);
            }

            //post play check last stand

            var automaticallyTriggeredResultTheLastStand = _automaticallyTriggeredCardEffectProcessors.First(x => x.CardAffected == CardValue.TheLastStand).ProcessCardEffect(game, string.Empty, new AutomaticallyTriggeredParams() { TheLastStandParams = new AutomaticallyTriggeredTheLastStandParams() });
            if (!string.IsNullOrEmpty(automaticallyTriggeredResultTheLastStand.MessageToLog))
                messagesToLog.Add(automaticallyTriggeredResultTheLastStand.MessageToLog);


            //add messages for the end game/round
            messagesToLog.AddRange(_gameManager.UpdateGameAndRoundStatus(game));

            //add messages outside play card processor to the move result
            moveResult.MessagesToLog.AddRange(messagesToLog);

            game.PlayerToPlay = _gameManager.GetNextPlayer(game, game.PlayerToPlay, game.Players);

            var automaticallyTriggeredResultQueensDecree = _automaticallyTriggeredCardEffectProcessors.First(x => x.CardAffected == CardValue.QueensDecree).ProcessCardEffect(game, string.Empty, new AutomaticallyTriggeredParams() { QueensDecreeParams = new AutomaticallyTriggeredQueensDecreeParams() { PlayerAffected = game.PlayerToPlay } });
            if (!string.IsNullOrEmpty(automaticallyTriggeredResultQueensDecree.MessageToLog))
                moveResult.MessagesToLog.Add(automaticallyTriggeredResultQueensDecree.MessageToLog);


            if (game.SilenceTurnsRemaining > 0)
            {
                game.SilenceTurnsRemaining--;
                moveResult.MessagesToLog.Add($"{game.SilenceTurnsRemaining} silenced turns remaining. ");
            }


            var result = game.GreedAffectedPlayers.TryGetValue(game.PlayerToPlay, out var greedTurns);
            if (result && greedTurns > 0)
            {
                if (game.SilenceTurnsRemaining <= 0)
                {
                    var messageToLog = $"{game.PlayerToPlay.User.Name} was affected by greed so they will draw one card. Greed turns remaining: {greedTurns - 1}. ";

                    if (game.PlayerToPlay.Cards.Count > 4 && game.PlayerToPlay.Cards.FirstOrDefault(x => x.Value == CardValue.KingsDecree) != null)
                    {
                        messageToLog += $"{game.PlayerToPlay.User.Name} is not affected by the draw (king's decree).";
                    }
                    else
                    {
                        var cardsToDraw = 1;
                        var doubleDrawCard = game.PlayerToPlay.Cards.FirstOrDefault(c => c.Value == CardValue.DoubleDraw);
                        if (doubleDrawCard != null)
                        {
                            game.LastCardPlayed = new LastCardPlayed(game.LastCardPlayed.Color, doubleDrawCard.Value, doubleDrawCard.ImageUrl, game.PlayerToPlay.User.Name, true, doubleDrawCard);
                            game.PlayerToPlay.Cards.Remove(doubleDrawCard);
                            game.DiscardedPile.Add(doubleDrawCard);
                            messageToLog += $"{game.PlayerToPlay.User.Name} doubled the draw/discard effect. ";
                            cardsToDraw = 2;
                        }
                        _gameManager.DrawCard(game, game.PlayerToPlay, cardsToDraw, false);
                    }
                    moveResult.MessagesToLog.Add(messageToLog);
                }
                game.GreedAffectedPlayers[game.PlayerToPlay] = greedTurns - 1;
            }


            return moveResult;
        }

        private bool IsValidMove(Game game, Player playerPlayed, ICard cardPlayed)
        {

            if (cardPlayed.Color != CardColor.Wild && cardPlayed.Color != game.LastCardPlayed.Color && cardPlayed.Value != game.LastCardPlayed.Value)
                return false;

            if (cardPlayed.Value == CardValue.StealTurn)
                return true;

            if (game.GameSetup.MatchingCardStealsTurn && cardPlayed.Color == game.LastCardPlayed.Color && cardPlayed.Value == game.LastCardPlayed.Value)
                return true;

            if (game.PlayerToPlay != playerPlayed)
            {
                return false;
            }

            if (cardPlayed.Color == CardColor.Wild && game.GameSetup.WildCardCanBePlayedOnlyIfNoOtherOptions)
            {
                if (playerPlayed.Cards.Any(x => x.Color == game.LastCardPlayed.Color) || (playerPlayed.Cards.Any(x => x.Value == game.LastCardPlayed.Value) && !game.LastCardPlayed.WasWildCard))
                {
                    return false;
                }
            }

            return true;
        }
    }
}