using System;
using System.Collections.Generic;
using Common.Enums;
using EntityObjects.Cards.Abstraction;

namespace EntityObjects.Cards.Wild
{
    public class Deflect : ICard
    {
        public Deflect(bool limitColorChangingCards)
        {
            Id = Guid.NewGuid().ToString();
            ImageUrl = $"/images/cards/small/{(int)Color}/{Convert.ToInt32(limitColorChangingCards)}/{(int)Value}.png";
        }
        public string Id { get; }
        public CardColor Color => CardColor.Wild;
        public CardValue Value => CardValue.Deflect;
        public string ImageUrl { get; }
        public bool RequirePickColor => true;
        public bool RequireTargetPlayer => false;
        public string Description => "This card is automatically played from your hand when another player targets you with the card that has a draw effects (double edge, +4, +2, judgement). As the result they must draw cards. Acts as normal wildcard otherwise.";

    }
}