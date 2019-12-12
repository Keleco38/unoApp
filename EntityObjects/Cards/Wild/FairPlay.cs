using System;
using System.Collections.Generic;
using System.Linq;
using Common.Enums;
using EntityObjects.Cards.Abstraction;

namespace EntityObjects.Cards.Wild
{
    public class FairPlay : ICard
    {
        public FairPlay(bool limitColorChangingCards)
        {
            Id = Guid.NewGuid().ToString();
            ImageUrl = $"/images/cards/small/{(int)Color}/{Convert.ToInt32(limitColorChangingCards)}/{(int)Value}.png";
        }
        public string Id { get; }
        public CardColor Color => CardColor.Wild;
        public CardValue Value => CardValue.FairPlay;
        public string ImageUrl { get; }
        public bool RequirePickColor => true;
        public bool RequireTargetPlayer => true;
        public string Description => "Target another player. If they have more cards than you, they discard until they have the same number as you. If they have less cards than you, they draw until the same number as you. Can be blocked by \"keep my hand\" card. Special effect is activated only if you have less than 10 cards in your hand (counter after the use). Otherwise acts as normal wild card.";
    }
}