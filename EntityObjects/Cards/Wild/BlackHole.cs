using System;
using System.Collections.Generic;
using System.Linq;
using Common.Enums;
using EntityObjects.Cards.Abstraction;

namespace EntityObjects.Cards.Wild
{
    public class BlackHole : ICard
    {
        public BlackHole(bool limitColorChangingCards)
        {
            Id = Guid.NewGuid().ToString();
            ImageUrl = $"/images/cards/small/{(int)Color}/{Convert.ToInt32(limitColorChangingCards)}/{(int)Value}.png";
        }
        public string Id { get; }
        public CardColor Color => CardColor.Wild;
        public CardValue Value => CardValue.BlackHole;
        public string ImageUrl { get; }
        public bool RequirePickColor => true;
        public bool RequireTargetPlayer => false;
        public string Description => "All cards in all hands are shuffled into a pile and then dealt to each person with the same number they had before. Can be blocked by \"keep my hand\" card.";
    }
}