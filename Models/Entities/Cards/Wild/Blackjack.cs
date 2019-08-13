using System;
using System.Collections.Generic;
using System.Linq;
using Uno.Enums;
using Uno.Models;
using unoApp.Models.Abstraction;
using unoApp.Models.Helpers;

namespace unoApp.Models.Entities.Cards.Wild
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
            var messageToLog = $"{moveParams.PlayerPlayed.User.Name} played blackjack. He/she hit {moveParams.BlackjackNumber}. ";
            if (moveParams.BlackjackNumber > 21)
            {
                game.DrawCard(moveParams.PlayerPlayed, 7, false);
                messageToLog += $"He/she went over 21. He/she will draw 7 cards.";
            }
            else if (moveParams.BlackjackNumber == 21)
            {
                var numberToDiscard = moveParams.PlayerPlayed.Cards.Count < 5 ? moveParams.PlayerPlayed.Cards.Count : 5;
                moveParams.PlayerPlayed.Cards.RemoveRange(0, numberToDiscard);
                messageToLog += $"He/she hit the blackjack. He/she will discard 5 cards.";

            }
            else if (moveParams.BlackjackNumber < 21 && moveParams.BlackjackNumber > 17)
            {

                var numberToDiscard = moveParams.PlayerPlayed.Cards.Count < 2 ? moveParams.PlayerPlayed.Cards.Count : 2;
                moveParams.PlayerPlayed.Cards.RemoveRange(0, numberToDiscard);
                messageToLog += $"He/she beat the dealer. He/she will discard 2 cards.";
            }
            else if (moveParams.BlackjackNumber == 17)
            {
                messageToLog += $"It's a draw. Nothing happens. ";
            }
            else
            {
                var difference = 17 - moveParams.BlackjackNumber;
                game.DrawCard(moveParams.PlayerPlayed, difference, false);
                messageToLog += $"He/she pulled out. He/she will draw {difference} cards.";
            }
            messagesToLog.Add(messageToLog);
           return new MoveResult(messagesToLog);
        }
    }
}