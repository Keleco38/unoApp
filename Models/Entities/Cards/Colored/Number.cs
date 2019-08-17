using System;
using System.Collections.Generic;
using Uno.Enums;
using Uno.Models.Entities.Cards.Abstraction;
using Uno.Models.Helpers;

namespace Uno.Models.Entities.Cards.Colored
{
    public class Number : ICard
    {
        public Number(CardColor cardColor, CardValue cardValue)
        {
            Id = Guid.NewGuid().ToString();
            Color = cardColor;
            Value = cardValue;
            ImageUrl = $"/images/cards/small/{(int)cardColor}/{(int)cardValue}.png";
        }
        public string Id { get; set; }
        public CardColor Color { get; set; }
        public CardValue Value { get; set; }
        public string ImageUrl { get; set; }

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {
            var messagesToLog = new List<string>();
            messagesToLog.Add($"{moveParams.PlayerPlayed.User.Name} played {Color.ToString()} {Value.ToString()}.");
           return new MoveResult(messagesToLog);
        }
    }
}