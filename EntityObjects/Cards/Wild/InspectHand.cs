using System;
using System.Collections.Generic;
using System.Linq;
using Common.Enums;
using EntityObjects.Cards.Abstraction;

namespace EntityObjects.Cards.Wild
{
    public class InspectHand : ICard
    {
        public InspectHand(bool limitColorChangingCards)
        {
            Id = Guid.NewGuid().ToString();
            ImageUrl = $"/images/cards/small/{(int)Color}/{Convert.ToInt32(limitColorChangingCards)}/{(int)Value}.png";
        }
        public string Id { get; }
        public CardColor Color => CardColor.Wild;
        public CardValue Value => CardValue.InspectHand;
        public string ImageUrl { get; }
        public bool RequirePickColor => true;
        public bool RequireTargetPlayer => true;
        public string Description => "Select a person. That person must show their hand to you.";
    }
}