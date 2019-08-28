using System;
using System.Collections.Generic;
using Common.Enums;
using EntityObjects.Cards.Abstraction;

namespace EntityObjects.Cards.Colored
{
    public class Number : ICard
    {
        public Number(CardColor cardColor, CardValue cardValue)
        {
            Id = Guid.NewGuid().ToString();
            Color = cardColor;
            Value = cardValue;
            ImageUrl = $"/images/cards/small/{(int)cardColor}/{(int)cardValue}.png";
        }
        public string Id { get; set; }
        public CardColor Color { get; set; }
        public CardValue Value { get; set; }
        public string ImageUrl { get; set; }

     
    }
}