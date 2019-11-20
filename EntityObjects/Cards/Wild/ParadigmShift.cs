using System;
using System.Collections.Generic;
using System.Linq;
using Common.Enums;
using EntityObjects.Cards.Abstraction;

namespace EntityObjects.Cards.Wild
{
    public class ParadigmShift : ICard
    {
        public ParadigmShift(bool limitColorChangingCards)
        {
            Id = Guid.NewGuid().ToString();
            ImageUrl = $"/images/cards/small/{(int)Color}/{Convert.ToInt32(limitColorChangingCards)}/{(int)Value}.png";
        }
        public string Id { get; }
        public CardColor Color => CardColor.Wild;
        public CardValue Value => CardValue.ParadigmShift;
        public string ImageUrl { get; }
        public bool RequirePickColor => true;
        public bool RequireTargetPlayer => false;
        public string Description => "Each person swaps hands to the person next to them according to the current direction of the game. Can be blocked by \"keep by hand\" card.";
    }
}