using System.Collections.Generic;
using System.Linq;
using Common.Enums;
using EntityObjects;
using GameProcessingService.CardEffectProcessors;
using GameProcessingService.CoreManagers.GameManagers;
using GameProcessingService.Models;

namespace GameProcessingService.CoreManagers.PlayCardManagers
{
    public class PlayCardManager : IPlayCardManager
    {
        private readonly IEnumerable<ICardEffectProcessor> _cardEffectProcessors;
        private readonly IGameManager _gameManager;

        public PlayCardManager(IEnumerable<ICardEffectProcessor> cardEffectProcessors, IGameManager gameManager)
        {
            _cardEffectProcessors = cardEffectProcessors;
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

            var extraMessageToLog = string.Empty;

            if (playerPlayed.CardPromisedToDiscard != null)
            {
                if (cardPlayed.Color == playerPlayed.CardPromisedToDiscard.Color && cardPlayed.Value == playerPlayed.CardPromisedToDiscard.Value)
                {
                    extraMessageToLog = $"{playerPlayed.User.Name} fulfilled their promise, they will discard one card. ";
                    if (playerPlayed.Cards.Count > 0)
                    {
                        playerPlayed.Cards.RemoveRange(0, 1);
                    }
                }
                else
                {
                    extraMessageToLog = $"{playerPlayed.User.Name} didn't fulfill their promise, they will draw 2 cards. ";
                    _gameManager.DrawCard(game, playerPlayed, 2, false);
                }
                playerPlayed.CardPromisedToDiscard = null;
            }

            var playerTargeted = string.IsNullOrEmpty(playerTargetedId) ? _gameManager.GetNextPlayer(game, playerPlayed, game.Players) : game.Players.Find(x => x.Id == playerTargetedId);
            var colorForLastCard = targetedCardColor == 0 ? cardPlayed.Color : targetedCardColor;

            game.LastCardPlayed = new LastCardPlayed(colorForLastCard, cardPlayed.Value, cardPlayed.ImageUrl, playerPlayed.User.Name, cardPlayed.Color == CardColor.Wild);

            var cardToDig = string.IsNullOrEmpty(cardToDigId) ? null : game.DiscardedPile.Find(x => x.Id == cardToDigId);
            var cardPromisedToDiscard = string.IsNullOrEmpty(cardPromisedToDiscardId) ? null : playerPlayed.Cards.Find(x => x.Id == cardPromisedToDiscardId);
            var charityCards = charityCardsIds != null ? playerPlayed.Cards.Where(x => charityCardsIds.Contains(x.Id)).ToList() : null;

            var moveParams = new MoveParams(playerPlayed,cardPlayed, playerTargeted, colorForLastCard, cardToDig, duelNumbers, charityCards, blackjackNumber, numbersToDiscard, cardPromisedToDiscard, oddOrEvenGuess);

            var targetedCardEffectProcessor = cardPlayed.GetType().Name.ToLower() + "effectprocessor";

            var cardEffectProcessor = _cardEffectProcessors.First(x => x.GetType().Name.ToLower().Equals(targetedCardEffectProcessor));

            var moveResult = cardEffectProcessor.ProcessCardEffect(game, moveParams);
            if (!string.IsNullOrEmpty(extraMessageToLog))
            {
                moveResult.MessagesToLog.Add(extraMessageToLog);
            }

            _gameManager.UpdateGameAndRoundStatus(game, moveResult);
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
    }
}