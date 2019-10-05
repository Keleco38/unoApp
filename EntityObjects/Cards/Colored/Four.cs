using System;
using System.Collections.Generic;
using Common.Enums;
using EntityObjects.Cards.Abstraction;

namespace EntityObjects.Cards.Colored
{
    public class Four : ICard
    {
        public Four(CardColor cardColor)
        {
            Id = Guid.NewGuid().ToString();
            Color = cardColor;
        }
        public string Id { get; set; }
        public CardColor Color { get; set; }
        public CardValue Value => CardValue.Four;
        public string ImageUrl => $"/images/cards/small/{(int)Color}/{(int)Value}.png";
        public bool RequirePickColor => false;
        public bool RequireTargetPlayer => false;
        public string Description => "Regular card (number four)";
    }
}