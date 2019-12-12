using System;
using System.Collections.Generic;
using System.Linq;
using Common.Enums;
using EntityObjects.Cards.Abstraction;

namespace EntityObjects.Cards.Wild
{
    public class FreshStart : ICard
    {
        public FreshStart(bool limitColorChangingCards)
        {
            Id = Guid.NewGuid().ToString();
            ImageUrl = $"/images/cards/small/{(int)Color}/{Convert.ToInt32(limitColorChangingCards)}/{(int)Value}.png";
        }
        public string Id { get; }
        public CardColor Color => CardColor.Wild;
        public CardValue Value => CardValue.FreshStart;
        public string ImageUrl { get; }
        public bool RequirePickColor => true;
        public bool RequireTargetPlayer => false;
        public string Description => "Player can choose to activate the special effect or not. If they activate the special effect of the card, they will discard their whole hand and draw 7 cards (like in the beginning of the game). If they decides not to activate the special effect, he must draw 1 card (not affected by double draw or king's decree). Can be blocked by \"keep my hand\".";
    }
}