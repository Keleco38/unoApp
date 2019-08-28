using System;
using System.Collections.Generic;
using System.Linq;
using Common.Enums;
using EntityObjects.Cards.Abstraction;

namespace EntityObjects.Cards.Wild
{
    public class UnitedWeFall : ICard
    {
        public UnitedWeFall()
        {
            Id = Guid.NewGuid().ToString();
            Color = CardColor.Wild;
            Value = CardValue.UnitedWeFall;
            ImageUrl = $"/images/cards/small/{(int)Color}/{(int)Value}.png";
        }
        public string Id { get; set; }
        public CardColor Color { get; set; }
        public CardValue Value { get; set; }
        public string ImageUrl { get; set; }
    }
}