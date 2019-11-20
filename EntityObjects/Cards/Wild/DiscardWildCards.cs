using System;
using System.Collections.Generic;
using System.Linq;
using Common.Enums;
using EntityObjects.Cards.Abstraction;

namespace EntityObjects.Cards.Wild
{
    public class DiscardWildCards : ICard
    {
        public DiscardWildCards(bool limitColorChangingCards)
        {
            Id = Guid.NewGuid().ToString();
            ImageUrl = $"/images/cards/small/{(int)Color}/{Convert.ToInt32(limitColorChangingCards)}/{(int)Value}.png";
            RequirePickColor = !limitColorChangingCards;
        }
        public string Id { get; }
        public CardColor Color => CardColor.Wild;
        public CardValue Value => CardValue.DiscardWildCards;
        public string ImageUrl { get; }
        public bool RequirePickColor { get; }
        public bool RequireTargetPlayer => false;
        public string Description => "At the cost of drawing one card every player will discard all wildcards from their hand.";

    }
}