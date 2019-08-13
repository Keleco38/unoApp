using System;
using System.Collections.Generic;
using System.Linq;
using Uno.Enums;
using Uno.Models;
using unoApp.Models.Abstraction;
using unoApp.Models.Helpers;

namespace unoApp.Models.Entities.Cards.Wild
{
    public class TheLastStand : ICard
    {
        public TheLastStand()
        {
            Id = Guid.NewGuid().ToString();
            Color = CardColor.Wild;
            Value = CardValue.TheLastStand;
            ImageUrl = $"/images/cards/small/{(int)Color}/{(int)Value}.png";
        }
        public string Id { get; set; }
        public CardColor Color { get; set; }
        public CardValue Value { get; set; }
        public string ImageUrl { get; set; }

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {
            var messagesToLog = new List<string>();
            messagesToLog.Add($"{moveParams.PlayerPlayed.User.Name} changed color to {moveParams.TargetedCardColor} (the last stand).");
           return new MoveResult(messagesToLog);
        }
    }
}