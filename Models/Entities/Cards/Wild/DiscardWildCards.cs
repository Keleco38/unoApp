using System;
using System.Collections.Generic;
using System.Linq;
using Uno.Enums;
using Uno.Models.Entities.Cards.Abstraction;
using Uno.Models.Helpers;

namespace Uno.Models.Entities.Cards.Wild
{
    public class DiscardWildCards : ICard
    {
        public DiscardWildCards()
        {
            Id = Guid.NewGuid().ToString();
            Color = CardColor.Wild;
            Value = CardValue.DiscardWildCards;
            ImageUrl = $"/images/cards/small/{(int)Color}/{(int)Value}.png";
        }
        public string Id { get; set; }
        public CardColor Color { get; set; }
        public CardValue Value { get; set; }
        public string ImageUrl { get; set; }

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {
            var messagesToLog = new List<string>();

            game.Players.ForEach(x =>
            {
                var wildCards = x.Cards.Where(y => y.Color == CardColor.Wild).ToList();
                game.DiscardedPile.AddRange(wildCards);
                wildCards.ForEach(y => x.Cards.Remove(y));
            });
            messagesToLog.Add($"{moveParams.PlayerPlayed.User.Name}  played discard all wildcards.");
            game.DrawCard(moveParams.PlayerPlayed, 1, false);

            return new MoveResult(messagesToLog);
        }
    }
}