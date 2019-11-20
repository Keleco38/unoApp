using System;
using System.Collections.Generic;
using Common.Enums;
using EntityObjects.Cards.Abstraction;

namespace EntityObjects.Cards.Wild
{
    public class Blackjack : ICard
    {
        public Blackjack(bool limitColorChangingCards)
        {
            Id = Guid.NewGuid().ToString();
            ImageUrl = $"/images/cards/small/{(int)Color}/{Convert.ToInt32(limitColorChangingCards)}/{(int)Value}.png";
            RequirePickColor = !limitColorChangingCards;
        }
        public string Id { get; }
        public CardColor Color => CardColor.Wild;
        public CardValue Value => CardValue.Blackjack;
        public string ImageUrl { get; }
        public bool RequirePickColor { get; }
        public bool RequireTargetPlayer => false;
        public string Description => "Player can hit the \"HIT ME!\" button to get a random number from 1 to 10. If the player goes over 21, they draw 5 cards. If the player hits 21, they will discard 3 cards. If the player hits number bigger than 17 but less than 21 they will discard 1 cards. If the player hits the number lower than 17, they will draw the difference between 17 and the number they hit.";

    }
}