using System.Collections.Generic;
using Common.Enums;
using System.Linq;
using EntityObjects;
using GameProcessingService.CoreManagers;
using GameProcessingService.Models;

namespace GameProcessingService.CardEffectProcessors.Wild
{
    public class BlackjackEffectProcessor : ICardEffectProcessor
    {
        private readonly IGameManager _gameManager;
        public CardValue CardAffected => CardValue.Blackjack;

        public BlackjackEffectProcessor(IGameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {
            var messagesToLog = new List<string>();
            var messageToLog = $"{moveParams.PlayerPlayed.User.Name} played blackjack. They hit {moveParams.BlackjackNumber}. ";
            if (moveParams.BlackjackNumber > 21)
            {
                var numberOfCardsToDraw = 5;
                var doubleDrawCard = moveParams.PlayerPlayed.Cards.FirstOrDefault(c => c.Value == CardValue.DoubleDraw);
                if (doubleDrawCard != null)
                {
                    game.LastCardPlayed = new LastCardPlayed(moveParams.TargetedCardColor, doubleDrawCard.Value, doubleDrawCard.ImageUrl, moveParams.PlayerPlayed.User.Name, true);
                    moveParams.PlayerPlayed.Cards.Remove(doubleDrawCard);
                    game.DiscardedPile.Add(doubleDrawCard);
                    numberOfCardsToDraw = numberOfCardsToDraw * 2;
                    messageToLog += $"{moveParams.PlayerPlayed.User.Name} doubled the draw effect. ";
                }

                _gameManager.DrawCard(game, moveParams.PlayerPlayed, numberOfCardsToDraw, false);
                messageToLog += $"They went over 21. They will draw {numberOfCardsToDraw} cards.";
            }
            else if (moveParams.BlackjackNumber == 21)
            {
                var numberToDiscard = moveParams.PlayerPlayed.Cards.Count < 3 ? moveParams.PlayerPlayed.Cards.Count : 3;
                moveParams.PlayerPlayed.Cards.RemoveRange(0, numberToDiscard);
                messageToLog += $"They hit the blackjack. They will discard 3 cards.";

            }
            else if (moveParams.BlackjackNumber < 21 && moveParams.BlackjackNumber > 17)
            {

                var numberToDiscard = moveParams.PlayerPlayed.Cards.Count < 1 ? moveParams.PlayerPlayed.Cards.Count : 1;
                moveParams.PlayerPlayed.Cards.RemoveRange(0, numberToDiscard);
                messageToLog += $"They beat the dealer. They will discard 1 card.";
            }
            else if (moveParams.BlackjackNumber == 17)
            {
                messageToLog += $"It's a draw. Nothing happens. ";
            }
            else
            {
                var numberOfCardsToDraw = 17 - moveParams.BlackjackNumber;
                var doubleDrawCard = moveParams.PlayerPlayed.Cards.FirstOrDefault(c => c.Value == CardValue.DoubleDraw);
                if (doubleDrawCard != null)
                {
                    game.LastCardPlayed = new LastCardPlayed(moveParams.TargetedCardColor, doubleDrawCard.Value, doubleDrawCard.ImageUrl, moveParams.PlayerPlayed.User.Name, true);
                    moveParams.PlayerPlayed.Cards.Remove(doubleDrawCard);
                    game.DiscardedPile.Add(doubleDrawCard);
                    numberOfCardsToDraw = numberOfCardsToDraw * 2;
                    messageToLog += $"{moveParams.PlayerPlayed.User.Name} doubled the draw effect. ";
                }

                _gameManager.DrawCard(game, moveParams.PlayerPlayed, numberOfCardsToDraw, false);
                messageToLog += $"They pulled out. They will draw {numberOfCardsToDraw} cards.";
            }
            messagesToLog.Add(messageToLog);
            return new MoveResult(messagesToLog);
        }
    }
}