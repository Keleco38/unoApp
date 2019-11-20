using System;
using System.Collections.Generic;
using Common.Enums;
using EntityObjects.Cards.Abstraction;

namespace EntityObjects.Cards.Wild
{
    public class DoubleDraw : ICard
    {
        public DoubleDraw(bool limitColorChangingCards)
        {
            Id = Guid.NewGuid().ToString();
            ImageUrl = $"/images/cards/small/{(int)Color}/{Convert.ToInt32(limitColorChangingCards)}/{(int)Value}.png";
        }
        public string Id { get; }
        public CardColor Color => CardColor.Wild;
        public CardValue Value => CardValue.DoubleDraw;
        public string ImageUrl { get; }
        public bool RequirePickColor => true;
        public bool RequireTargetPlayer => false;
        public string Description => "Player can play this card only if the last card played was a wildcard. If someone targets you with a wild card that has a draw effect, this card is played automatically and the player will double the draw effect. This card's effect is activated after magnetic polarity but before deflect. That means that if the player has magnetic polarity, double draw and deflect, they will first intercept the attack with the magnetic polarity, then double the draw effect, and then deflect it to the caller.";
    }
}