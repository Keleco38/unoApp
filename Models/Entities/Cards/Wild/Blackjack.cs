using System;
using System.Collections.Generic;
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
                game.DrawCard(moveParams.PlayerPlayed, 6, false);
                messageToLog += $"They went over 21. They will draw 6 cards.";
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
                var difference = 17 - moveParams.BlackjackNumber;
                game.DrawCard(moveParams.PlayerPlayed, difference, false);
                messageToLog += $"They pulled out. They will draw {difference} cards.";
            }
            messagesToLog.Add(messageToLog);
           return new MoveResult(messagesToLog);
        }
    }
}