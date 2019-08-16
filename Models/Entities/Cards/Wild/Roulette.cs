using System;
using System.Collections.Generic;
using Uno.Enums;
using Uno.Models.Entities.Cards.Abstraction;
using Uno.Models.Helpers;

namespace Uno.Models.Entities.Cards.Wild
{
    public class Roulette : ICard
    {
        public Roulette()
        {
            Id = Guid.NewGuid().ToString();
            Color = CardColor.Wild;
            Value = CardValue.Roulette;
            ImageUrl = $"/images/cards/small/{(int)Color}/{(int)Value}.png";
        }
        public string Id { get; set; }
        public CardColor Color { get; set; }
        public CardValue Value { get; set; }
        public string ImageUrl { get; set; }

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {
            var messagesToLog = new List<string>();
            Random random = new Random();
            var messageToLog = $"{moveParams.PlayerPlayed.User.Name} played Roulette. ";
            var drawOrDiscard = random.Next(2);
            var playerAffected = game.Players[random.Next(game.Players.Count)];
            if (drawOrDiscard == 0)
            {
                //discard   

                var maxNumberToDiscard = playerAffected.Cards.Count < 3 ? playerAffected.Cards.Count + 1 : 3;
                var numberOfCardsToDiscard = random.Next(0, maxNumberToDiscard);
                if (numberOfCardsToDiscard == 0)
                {
                    messageToLog += $"{playerAffected.User.Name} was selected, but They won't discard not draw any cards.";
                }
                else
                {
                    playerAffected.Cards.RemoveRange(0, numberOfCardsToDiscard);
                    messageToLog += $"{playerAffected.User.Name} is a lucky winner! They will discard {numberOfCardsToDiscard} cards.";
                }
            }
            else
            {
                //draw
                var numberOfCardsToDraw = random.Next(1, 5);
                messageToLog += $"{playerAffected.User.Name} didn't have any luck! They will draw {numberOfCardsToDraw} cards.";
                game.DrawCard(playerAffected, numberOfCardsToDraw, false);
            }

            messagesToLog.Add(messageToLog);
           return new MoveResult(messagesToLog);
        }
    }
}