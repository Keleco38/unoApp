using System;
using System.Collections.Generic;
using Common.Enums;
using EntityObjects.Cards.Abstraction;

namespace EntityObjects.Cards.Wild
{
    public class KeepMyHand : ICard
    {
        public KeepMyHand(bool limitColorChangingCards)
        {
            Id = Guid.NewGuid().ToString();         
            ImageUrl = $"/images/cards/small/{(int)Color}/{Convert.ToInt32(limitColorChangingCards)}/{(int)Value}.png";
        }
        public string Id { get; }
        public CardColor Color => CardColor.Wild;
        public CardValue Value => CardValue.KeepMyHand;
        public string ImageUrl { get; }
        public bool RequirePickColor => true;
        public bool RequireTargetPlayer => false;
        public string Description => "This card is automatically played from your hand in case of \"black hole\", \"swap hands\", \"fair play\" or \"paradigm shift\". Your cards remains intact.";
    }
}