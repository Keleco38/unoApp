using System;
using System.Collections.Generic;
using Uno.Enums;
using Uno.Models.Entities.Cards.Abstraction;
using Uno.Models.Helpers;

namespace Uno.Models.Entities.Cards.Wild
{
    public class UnitedWeFall : ICard
    {
        public UnitedWeFall()
        {
            Id = Guid.NewGuid().ToString();
            Color = CardColor.Wild;
            Value = CardValue.UnitedWeFall;
            ImageUrl = $"/images/cards/small/{(int)Color}/{(int)Value}.png";
        }
        public string Id { get; set; }
        public CardColor Color { get; set; }
        public CardValue Value { get; set; }
        public string ImageUrl { get; set; }

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {
            var messagesToLog = new List<string>();
            game.Players.ForEach(x => game.DrawCard(x, 2, false));
            messagesToLog.Add($"{moveParams.PlayerPlayed.User.Name}  played united we fall card. Every player drew 2 cards.");
            return new MoveResult(messagesToLog);
        }
    }
}