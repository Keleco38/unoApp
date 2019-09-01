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
            var cardPlayed = playerPlayed.Cards.Find(x => x.Id == cardPlayedId);

            if (game.PlayerToPlay != playerPlayed && cardPlayed.Value != CardValue.StealTurn)
                return null;
            if (cardPlayed.Color != CardColor.Wild && cardPlayed.Color != game.LastCardPlayed.Color && cardPlayed.Value != game.LastCardPlayed.Value)
                return null;

            playerPlayed.Cards.Remove(cardPlayed);
            game.DiscardedPile.Add(cardPlayed);



            var playerTargeted = string.IsNullOrEmpty(playerTargetedId) ? _gameManager.GetNextPlayer(game, playerPlayed, game.Players) : game.Players.Find(x => x.Id == playerTargetedId);
            var colorForLastCard = targetedCardColor == 0 ? cardPlayed.Color : targetedCardColor;

            game.LastCardPlayed = new LastCardPlayed(colorForLastCard, cardPlayed.Value, cardPlayed.ImageUrl, playerPlayed.User.Name, cardPlayed.Color == CardColor.Wild);

            var cardToDig = string.IsNullOrEmpty(cardToDigId) ? null : game.DiscardedPile.Find(x => x.Id == cardToDigId);
            var cardPromisedToDiscard = string.IsNullOrEmpty(cardPromisedToDiscardId) ? null : playerPlayed.Cards.Find(x => x.Id == cardPromisedToDiscardId);
            var charityCards = charityCardsIds != null ? playerPlayed.Cards.Where(x => charityCardsIds.Contains(x.Id)).ToList() : null;

            var moveParams = new MoveParams(playerPlayed, cardPlayed, playerTargeted, colorForLastCard, cardToDig, duelNumbers, charityCards, blackjackNumber, numbersToDiscard, cardPromisedToDiscard, oddOrEvenGuess);

            var automaticallyTriggeredResultPromiseKeeper = _automaticallyTriggeredCardEffectProcessors.First(x => x.CardAffected == CardValue.PromiseKeeper).ProcessCardEffect(game, new AutomaticallyTriggeredParams(moveParams, String.Empty, null, 0));

            var moveResult = _playableCardEffectProcessors.First(x => x.CardAffected == cardPlayed.Value).ProcessCardEffect(game, moveParams);

            if (!string.IsNullOrEmpty(automaticallyTriggeredResultPromiseKeeper.MessageToLog))
            {
                moveResult.MessagesToLog.Add(automaticallyTriggeredResultPromiseKeeper.MessageToLog);
            }

            UpdateGameAndRoundStatus(game, moveResult, moveParams);
            if (game.GameEnded)
            {
                return moveResult;
            }
            if (game.RoundEnded)
            {
                _gameManager.StartNewGame(game);
                return moveResult;
            }

            game.PlayerToPlay = _gameManager.GetNextPlayer(game, game.PlayerToPlay, game.Players);
            return moveResult;
        }


        // -------------------------------------private------------

        public void UpdateGameAndRoundStatus(Game game, MoveResult moveResult, MoveParams moveParams)
        {
            var playersWithoutCards = game.Players.Where(x => !x.Cards.Any()).ToList();
            if (playersWithoutCards.Any())
            {
                var automaticallyTriggeredResultTheLastStand = _automaticallyTriggeredCardEffectProcessors.First(x => x.CardAffected == CardValue.TheLastStand).ProcessCardEffect(game, new AutomaticallyTriggeredParams(moveParams, String.Empty, null, 0));

                if (!string.IsNullOrEmpty(automaticallyTriggeredResultTheLastStand.MessageToLog))
                {
                    return;
                }

                foreach (var player in playersWithoutCards)
                {
                    player.RoundsWonCount++;
                }

                game.RoundEnded = true;
                moveResult.MessagesToLog.Add($"Round ended! Players that won that round: {string.Join(',', playersWithoutCards.Select(x => x.User.Name))}");

                var playersThatMatchWinCriteria = game.Players.Where(x => x.RoundsWonCount == game.GameSetup.RoundsToWin).ToList();
                if (playersThatMatchWinCriteria.Any())
                {
                    game.GameEnded = true;
                    moveResult.MessagesToLog.Add($"Game ended! Players that won the game: {string.Join(',', playersThatMatchWinCriteria.Select(x => x.User.Name))}");
                }
            }
 
        }
    }
}