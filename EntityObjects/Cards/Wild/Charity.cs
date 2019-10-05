using System;
using System.Collections.Generic;
using System.Linq;
using Common.Enums;
using EntityObjects.Cards.Abstraction;

namespace EntityObjects.Cards.Wild
{
    public class Charity : ICard
    {
        public Charity()
        {
            Id = Guid.NewGuid().ToString();
        }
        public string Id { get; }
        public CardColor Color => CardColor.Wild;
        public CardValue Value => CardValue.Charity;
        public string ImageUrl => $"/images/cards/small/{(int)Color}/{(int)Value}.png";
        public bool RequirePickColor => true;
        public bool RequireTargetPlayer => true;
        public string Description => "At the cost of drawing one card target a player and select two cards from your hand. The targeted player will receive this two cards as a gift. Cannot be deflected. Cannot be played if the number of the cards in your hand is less than 3 .";

    }
}