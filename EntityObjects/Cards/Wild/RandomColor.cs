using System;
using System.Collections.Generic;
using Common.Enums;
using EntityObjects.Cards.Abstraction;

namespace EntityObjects.Cards.Wild
{
    public class RandomColor : ICard
    {
        public RandomColor(bool limitColorChangingCards)
        {
            Id = Guid.NewGuid().ToString();
            ImageUrl = $"/images/cards/small/{(int)Color}/{Convert.ToInt32(limitColorChangingCards)}/{(int)Value}.png";
        }
        public string Id { get; }
        public CardColor Color => CardColor.Wild;
        public CardValue Value => CardValue.RandomColor;
        public string ImageUrl { get; }
        public bool RequirePickColor => false;
        public bool RequireTargetPlayer => false;
        public string Description => "Random color is assigned.";

    }
}