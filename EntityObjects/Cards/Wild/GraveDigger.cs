using System;
using System.Collections.Generic;
using Common.Enums;
using EntityObjects.Cards.Abstraction;

namespace EntityObjects.Cards.Wild
{
    public class GraveDigger : ICard
    {
        public GraveDigger(bool limitColorChangingCards)
        {
            Id = Guid.NewGuid().ToString();
            ImageUrl = $"/images/cards/small/{(int)Color}/{Convert.ToInt32(limitColorChangingCards)}/{(int)Value}.png";
            RequirePickColor = !limitColorChangingCards;
        }
        public string Id { get; }
        public CardColor Color => CardColor.Wild;
        public CardValue Value => CardValue.GraveDigger;
        public string ImageUrl { get; }
        public bool RequirePickColor { get; }
        public bool RequireTargetPlayer => false;
        public string Description => "Dig one card from the discarded pile.";
    }
}