using System;
using System.Collections.Generic;
using Uno.Enums;
using Uno.Models.Entities.Cards.Abstraction;
using Uno.Models.Helpers;

namespace Uno.Models.Entities.Cards.Wild
{
    public class GraveDigger : ICard
    {
        public GraveDigger()
        {
            Id = Guid.NewGuid().ToString();
            Color = CardColor.Wild;
            Value = CardValue.GraveDigger;
            ImageUrl = $"/images/cards/small/{(int)Color}/{(int)Value}.png";
        }
        public string Id { get; set; }
        public CardColor Color { get; set; }
        public CardValue Value { get; set; }
        public string ImageUrl { get; set; }

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {
            var messagesToLog = new List<string>();
            moveParams.PlayerPlayed.Cards.Add(moveParams.CardToDig);
            game.DiscardedPile.Remove(moveParams.CardToDig);
            messagesToLog.Add($"{moveParams.PlayerPlayed.User.Name} dug card {moveParams.CardToDig.Color.ToString()} {moveParams.CardToDig.Value.ToString()} from the discarded pile.");
           return new MoveResult(messagesToLog);
        }
    }
}