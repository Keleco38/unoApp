using System;
using System.Collections.Generic;
using Common.Enums;
using EntityObjects.Cards.Abstraction;

namespace EntityObjects.Cards.Wild
{
    public class MagneticPolarity : ICard
    {
        public MagneticPolarity()
        {
            Id = Guid.NewGuid().ToString();
        }
        public string Id { get; }
        public CardColor Color => CardColor.Wild;
        public CardValue Value => CardValue.MagneticPolarity;
        public string ImageUrl => $"/images/cards/small/{(int)Color}/{(int)Value}.png";
        public bool RequirePickColor => true;
        public bool RequireTargetPlayer => false;
        public string Description => "Player can play this card only if the last card played was a wildcard. If someone activates a wild card that requires another player as a target, this card is played automatically and the player will intercept the attack and become the targeted player. This card's effect is activated before deflect card. That means if the player has magnetic polarity and deflect card, they will first intercept the attack, and then deflect it back to the caller.";
    }
}