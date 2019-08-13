using System;
using System.Collections.Generic;
using Uno.Enums;
using Uno.Models.Entities.Cards.Abstraction;
using Uno.Models.Helpers;

namespace Uno.Models.Entities.Cards.Colored
{
    public class Skip : ICard
    {
        public Skip(CardColor cardColor)
        {
            Id = Guid.NewGuid().ToString();
            Color = cardColor;
            Value = CardValue.Skip;
            ImageUrl = $"/images/cards/small/{(int)cardColor}/{(int)Value}.png";
        }
        public string Id { get; set; }
        public CardColor Color { get; set; }
        public CardValue Value { get; set; }
        public string ImageUrl { get; set; }

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {
            var messagesToLog = new List<string>();
            messagesToLog.Add($"{moveParams.PlayerPlayed.User.Name} played skip turn. {moveParams.PlayerTargeted.User.Name} was skipped");
            game.PlayerToPlay = moveParams.PlayerTargeted;
           return new MoveResult(messagesToLog);
        }
    }
}