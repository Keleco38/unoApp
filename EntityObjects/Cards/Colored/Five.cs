using System;
using System.Collections.Generic;
using Common.Enums;
using EntityObjects.Cards.Abstraction;

namespace EntityObjects.Cards.Colored
{
    public class Five : ICard
    {
        public Five(CardColor cardColor)
        {
            Id = Guid.NewGuid().ToString();
            Color = cardColor;
        }
        public string Id { get; set; }
        public CardColor Color { get; set; }
        public CardValue Value => CardValue.Five;
        public string ImageUrl => $"/images/cards/small/{(int)Color}/{(int)Value}.png";
        public bool RequirePickColor => false;
        public bool RequireTargetPlayer => false;
    }
}