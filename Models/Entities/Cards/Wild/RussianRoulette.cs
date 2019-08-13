using System;
using System.Collections.Generic;
using Uno.Enums;
using Uno.Models.Entities.Cards.Abstraction;
using Uno.Models.Helpers;

namespace Uno.Models.Entities.Cards.Wild
{
    public class RussianRoulette : ICard
    {
        public RussianRoulette()
        {
            Id = Guid.NewGuid().ToString();
            Color = CardColor.Wild;
            Value = CardValue.RussianRoulette;
            ImageUrl = $"/images/cards/small/{(int)Color}/{(int)Value}.png";
        }
        public string Id { get; set; }
        public CardColor Color { get; set; }
        public CardValue Value { get; set; }
        public string ImageUrl { get; set; }

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {
            var messagesToLog = new List<string>();
            var messageToLog = $"{moveParams.PlayerPlayed.User.Name}  played Russian Roulette. Every player rolled a dice.";
            var playerRolling = moveParams.PlayerTargeted;
            Random random = new Random();

            while (true)
            {
                int rolledNumber = random.Next(1, 7);
                messageToLog += $" [{playerRolling.User.Name}: {rolledNumber}] ";
                if (rolledNumber == 1)
                {
                    messageToLog += $" {playerRolling.User.Name} drew 6 cards.";
                    game.DrawCard(playerRolling, 6, false);
                    break;
                }
                playerRolling = game.GetNextPlayer(playerRolling, game.Players);
            }
            messagesToLog.Add(messageToLog);
           return new MoveResult(messagesToLog);
        }
    }
}