using System;
using System.Collections.Generic;
using Common.Enums;
using EntityObjects.Cards.Abstraction;

namespace EntityObjects.Cards.Colored
{
    public class Reverse : ICard
    {
        public Reverse(CardColor cardColor)
        {
            Id = Guid.NewGuid().ToString();
            Color = cardColor;
            Value = CardValue.Reverse;
            ImageUrl = $"/images/cards/small/{(int)cardColor}/{(int)Value}.png";
        }
        public string Id { get; set; }
        public CardColor Color { get; set; }
        public CardValue Value { get; set; }
        public string ImageUrl { get; set; }


    }
}