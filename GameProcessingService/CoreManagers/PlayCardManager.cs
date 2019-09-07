using System;
using System.Collections.Generic;
using System.Linq;
using Common.Enums;
using EntityObjects;
using GameProcessingService.CardEffectProcessors;
using GameProcessingService.CardEffectProcessors.AutomaticallyTriggered;
using GameProcessingService.CardEffectProcessors.Played;
using GameProcessingService.Models;

namespace GameProcessingService.CoreManagers
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
           List<string> charityCardsIds, int blackjackNumber, List<int> numbersToDiscard, string cardPromisedToDiscardId, string oddOrEvenGuess)
        {
            var cardPlayed = playerPlayed.Cards.FirstOrDefault(x => x.Id == cardPlayedId);

            if (cardPlayed == null)
                return null;
            if (game.PlayerToPlay != playerPlayed && cardPlayed.Value != CardValue.StealTurn)
                return null;
            if (cardPlayed.Color != CardColor.Wild && cardPlayed.Color != game.LastCardPlayed.Color && cardPlayed.Value != game.LastCardPlayed.Value)
                return null;

            playerPlayed.Cards.Remove(cardPlayed);
            game.DiscardedPile.Add(cardPlayed);

            var playerTargeted = string.IsNullOrEmpty(playerTargetedId) ? _gameManager.GetNextPlayer(game, playerPlayed, game.Players) : game.Players.First(x => x.Id == playerTargetedId);
            var colorForLastCard = targetedCardColor == 0 ? cardPlayed.Color : targetedCardColor;

            game.LastCardPlayed = new LastCardPlayed(colorForLastCard, cardPlayed.Value, cardPlayed.ImageUrl, playerPlayed.User.Name, cardPlayed.Color == CardColor.Wild);

            var cardToDig = string.IsNullOrEmpty(cardToDigId) ? null : game.DiscardedPile.First(x => x.Id == cardToDigId);
            var cardPromisedToDiscard = string.IsNullOrEmpty(cardPromisedToDiscardId) ? null : playerPlayed.Cards.First(x => x.Id == cardPromisedToDiscardId);
            var charityCards = charityCardsIds != null ? playerPlayed.Cards.Where(x => charityCardsIds.Contains(x.Id)).ToList() : null;


            var automaticallyTriggeredResultPromiseKeeper = _automaticallyTriggeredCardEffectProcessors.First(x => x.CardAffected == CardValue.PromiseKeeper).ProcessCardEffect(game, string.Empty, new AutomaticallyTriggeredParams() { PromiseKeeperParams = new AutomaticallyTriggeredPromiseKeeperParams(playerPlayed, cardPlayed) });
            var turnMessageResult = automaticallyTriggeredResultPromiseKeeper.MessageToLog;

            var moveParams = new MoveParams(playerPlayed, cardPlayed, playerTargeted, colorForLastCard, cardToDig, duelNumbers, charityCards, blackjackNumber, numbersToDiscard, cardPromisedToDiscard, oddOrEvenGuess);

            var moveResult = _playableCardEffectProcessors.First(x => x.CardAffected == cardPlayed.Value).ProcessCardEffect(game, moveParams, turnMessageResult);

            var automaticallyTriggeredTheLastStandResult = _automaticallyTriggeredCardEffectProcessors.First(x => x.CardAffected == CardValue.TheLastStand).ProcessCardEffect(game, moveResult.TurnMessageResult, new AutomaticallyTriggeredParams() { TheLastStandParams = new AutomaticallyTriggeredTheLastStandParams() });
            moveResult.TurnMessageResult = automaticallyTriggeredTheLastStandResult.MessageToLog;


            _gameManager.UpdateGameAndRoundStatus(game);

            if (game.RoundEnded)
            {
                moveResult.RoundEndedMessageResult = $"Round ended! Players that won that round: {string.Join(',', game.Players.Where(x => !x.Cards.Any()).Select(x => x.User.Name))}";
                if (game.GameEnded)
                {
                    moveResult.GameEndedMessageResult = $"Game ended! Players that won the game: {string.Join(',', game.Players.Where(x => x.RoundsWonCount == game.GameSetup.RoundsToWin).Select(x => x.User.Name))}";
                }
                else
                {
                    _gameManager.StartNewGame(game);
                }
            }

            game.PlayerToPlay = _gameManager.GetNextPlayer(game, game.PlayerToPlay, game.Players);
            return moveResult;
        }


    }
}