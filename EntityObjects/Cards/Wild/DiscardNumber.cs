using System;
using System.Collections.Generic;
using System.Linq;
using Common.Enums;
using EntityObjects.Cards.Abstraction;

namespace EntityObjects.Cards.Wild
{
    public class DiscardNumber : ICard
    {
        public DiscardNumber(bool limitColorChangingCards)
        {
            Id = Guid.NewGuid().ToString();
            ImageUrl = $"/images/cards/small/{(int)Color}/{Convert.ToInt32(limitColorChangingCards)}/{(int)Value}.png";
            RequirePickColor = !limitColorChangingCards;
        }
        public string Id { get; }
        public CardColor Color => CardColor.Wild;
        public CardValue Value => CardValue.DiscardNumber;
        public string ImageUrl { get; }
        public bool RequirePickColor { get; }
        public bool RequireTargetPlayer => false;
        public string Description => "At the cost of drawing one card player selects 2 numbers (from 0-9) every player will discard these numbers from their hand, regardless the color.";

    }
}