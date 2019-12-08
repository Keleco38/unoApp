using System.Collections.Generic;
using System.Linq;
using Common.Enums;
using EntityObjects;
using GameProcessingService.CoreManagers;
using GameProcessingService.Models;

namespace GameProcessingService.CardEffectProcessors.Played.Wild
{
    public class RobinHoodEffectProcessor : IPlayedCardEffectProcessor
    {
        private readonly IGameManager _gameManager;
        public CardValue CardAffected => CardValue.RobinHood;

        public RobinHoodEffectProcessor(IGameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {
            var messagesToLog = new List<string>();
            var playerWithMostCards = game.Players.First(y => y.Cards.Count == game.Players.Max(p => p.Cards.Count));
            var playerWIthLeastCards = game.Players.First(y => y.Cards.Count == game.Players.Min(p => p.Cards.Count));

            var numberOfCardsToTake = playerWithMostCards.Cards.Count < 3 ? playerWithMostCards.Cards.Count : 3;

            var cards = playerWithMostCards.Cards.GetRange(0, numberOfCardsToTake);
            playerWithMostCards.Cards.RemoveRange(0, numberOfCardsToTake);

            var messageToLog = $"{moveParams.PlayerPlayed.User.Name} played RobinHood. They stole {numberOfCardsToTake} cards from {playerWithMostCards.User.Name}, ";


            if (numberOfCardsToTake == 3)
            {
                messageToLog += $"kept one for themselves and gave 2 to {playerWIthLeastCards.User.Name}. ";
                moveParams.PlayerPlayed.Cards.Add(cards[0]);
                playerWIthLeastCards.Cards.Add(cards[1]);
                playerWIthLeastCards.Cards.Add(cards[2]);
            }
            else if (numberOfCardsToTake == 2)
            {
                messageToLog += $"kept one for themselves and gave 1 to {playerWIthLeastCards.User.Name} ";
                moveParams.PlayerPlayed.Cards.Add(cards[0]);
                playerWIthLeastCards.Cards.Add(cards[1]);
            }
            else if (numberOfCardsToTake == 1)
            {
                messageToLog += $"and gave 1 to {playerWIthLeastCards.User.Name} ";
                playerWIthLeastCards.Cards.Add(cards[0]);
            }

            messagesToLog.Add(messageToLog);
            return new MoveResult(messagesToLog);
        }
    }
}