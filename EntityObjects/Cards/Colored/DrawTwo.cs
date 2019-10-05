using System;
using System.Collections.Generic;
using System.Linq;
using Common.Enums;
using EntityObjects.Cards.Abstraction;

namespace EntityObjects.Cards.Colored
{
    public class DrawTwo : ICard
    {
        public DrawTwo(CardColor cardColor)
        {
            Id = Guid.NewGuid().ToString();
            Color = cardColor;
        }
        public string Id { get; }
        public CardColor Color { get; }
        public CardValue Value => CardValue.DrawTwo;
        public string ImageUrl => $"/images/cards/small/{(int)Color}/{(int)Value}.png";
        public bool RequirePickColor => false;
        public bool RequireTargetPlayer => false;
        public string Description => "Next player will draw 2 cards. Can be deflected.";

    }
}