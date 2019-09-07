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
            var cardPlayed = playerPlayed.Cards.First(x => x.Id == cardPlayedId);

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

            var messagesToLog = new List<string>();

            //pre play check promise keeper
            var automaticallyTriggeredResultPromiseKeeper = _automaticallyTriggeredCardEffectProcessors.First(x => x.CardAffected == CardValue.PromiseKeeper).ProcessCardEffect(game, string.Empty, new AutomaticallyTriggeredParams() { PromiseKeeperParams = new AutomaticallyTriggeredPromiseKeeperParams(playerPlayed, cardPlayed) });
            if (!string.IsNullOrEmpty(automaticallyTriggeredResultPromiseKeeper.MessageToLog))
                messagesToLog.Add(automaticallyTriggeredResultPromiseKeeper.MessageToLog);

            //play card effect
            var moveParams = new MoveParams(playerPlayed, cardPlayed, playerTargeted, colorForLastCard, cardToDig, duelNumbers, charityCards, blackjackNumber, numbersToDiscard, cardPromisedToDiscard, oddOrEvenGuess);
            var moveResult = _playableCardEffectProcessors.First(x => x.CardAffected == cardPlayed.Value).ProcessCardEffect(game, moveParams);

            //post play check last stand
            var automaticallyTriggeredResultTheLastStand = _automaticallyTriggeredCardEffectProcessors.First(x => x.CardAffected == CardValue.TheLastStand).ProcessCardEffect(game, string.Empty, new AutomaticallyTriggeredParams() { TheLastStandParams = new AutomaticallyTriggeredTheLastStandParams() });
            if (!string.IsNullOrEmpty(automaticallyTriggeredResultTheLastStand.MessageToLog))
                messagesToLog.Add(automaticallyTriggeredResultTheLastStand.MessageToLog);

            //add messages for the end game/round
            messagesToLog.AddRange(_gameManager.UpdateGameAndRoundStatus(game));

            //add messages outside play card processor to the move result
            moveResult.MessagesToLog.AddRange(messagesToLog);

            game.PlayerToPlay = _gameManager.GetNextPlayer(game, game.PlayerToPlay, game.Players);
            return moveResult;
        }


    }
}