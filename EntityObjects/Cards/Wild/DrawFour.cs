using System;
using System.Collections.Generic;
using System.Linq;
using Common.Enums;
using EntityObjects.Cards.Abstraction;

namespace EntityObjects.Cards.Wild
{
    public class DrawFour : ICard
    {
        public DrawFour(bool limitColorChangingCards)
        {
            Id = Guid.NewGuid().ToString();
            ImageUrl = $"/images/cards/small/{(int)Color}/{Convert.ToInt32(limitColorChangingCards)}/{(int)Value}.png";
        }
        public string Id { get; }
        public CardColor Color => CardColor.Wild;
        public CardValue Value => CardValue.DrawFour;
        public string ImageUrl { get; }
        public bool RequirePickColor => true;
        public bool RequireTargetPlayer => false;
        public string Description => "Next player must draw four cards. Can be deflected.";

    }
}