using System;
using System.Collections.Generic;
using Common.Enums;
using EntityObjects.Cards.Abstraction;

namespace EntityObjects.Cards.Wild
{
    public class TheLastStand : ICard
    {
        public TheLastStand()
        {
            Id = Guid.NewGuid().ToString();
        }
        public string Id { get; }
        public CardColor Color => CardColor.Wild;
        public CardValue Value => CardValue.TheLastStand;
        public string ImageUrl => $"/images/cards/small/{(int)Color}/{(int)Value}.png";
        public bool RequirePickColor => true;
        public bool RequireTargetPlayer => false;
        public string Description => "If someone places their last card when a person has this card in their hand, this card will be automatically played and that person's win will be negated by drawing 2 new cards. Acts as normal wildcard otherwise.";

    }
}