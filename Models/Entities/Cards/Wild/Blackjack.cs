using System;
using System.Collections.Generic;
using System.Linq;
using Uno.Enums;
using Uno.Models.Entities.Cards.Abstraction;
using Uno.Models.Helpers;

namespace Uno.Models.Entities.Cards.Wild
{
    public class Blackjack : ICard
    {
        public Blackjack()
        {
            Id = Guid.NewGuid().ToString();
            Color = CardColor.Wild;
            Value = CardValue.Blackjack;
            ImageUrl = $"/images/cards/small/{(int)Color}/{(int)Value}.png";
        }
        public string Id { get; set; }
        public CardColor Color { get; set; }
        public CardValue Value { get; set; }
        public string ImageUrl { get; set; }

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {
            var messagesToLog = new List<string>();
            var messageToLog = $"{moveParams.PlayerPlayed.User.Name} played blackjack. They hit {moveParams.BlackjackNumber}. ";
            if (moveParams.BlackjackNumber > 21)
            {
                var numberOfCardsToDraw = 6;
                var doubleDrawCard = moveParams.PlayerPlayed.Cards.FirstOrDefault(c => c.Value == CardValue.DoubleDraw);
                if (doubleDrawCard != null)
                {
                    game.LastCardPlayed = new LastCardPlayed(moveParams.TargetedCardColor, doubleDrawCard.Value, doubleDrawCard.ImageUrl, moveParams.PlayerPlayed.User.Name, true);
                    moveParams.PlayerPlayed.Cards.Remove(doubleDrawCard);
                    game.DiscardedPile.Add(doubleDrawCard);
                    numberOfCardsToDraw = numberOfCardsToDraw * 2;
                    messageToLog += $"{moveParams.PlayerPlayed.User.Name} doubled the draw effect. ";
                }

                game.DrawCard(moveParams.PlayerPlayed, numberOfCardsToDraw, false);
                messageToLog += $"They went over 21. They will draw {numberOfCardsToDraw} cards.";
            }
            else if (moveParams.BlackjackNumber == 21)
            {
                var numberToDiscard = moveParams.PlayerPlayed.Cards.Count < 4 ? moveParams.PlayerPlayed.Cards.Count : 4;
                moveParams.PlayerPlayed.Cards.RemoveRange(0, numberToDiscard);
                messageToLog += $"They hit the blackjack. They will discard 4 cards.";

            }
            else if (moveParams.BlackjackNumber < 21 && moveParams.BlackjackNumber > 17)
            {

                var numberToDiscard = moveParams.PlayerPlayed.Cards.Count < 2 ? moveParams.PlayerPlayed.Cards.Count : 2;
                moveParams.PlayerPlayed.Cards.RemoveRange(0, numberToDiscard);
                messageToLog += $"They beat the dealer. They will discard 2 cards.";
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

                game.DrawCard(moveParams.PlayerPlayed, numberOfCardsToDraw, false);
                messageToLog += $"They pulled out. They will draw {numberOfCardsToDraw} cards.";
            }
            messagesToLog.Add(messageToLog);
            return new MoveResult(messagesToLog);
        }
    }
}