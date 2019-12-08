using System;
using Common.Enums;
using EntityObjects.Cards.Abstraction;

namespace EntityObjects.Cards.Wild
{
    public class KingsDecree : ICard
    {
        public KingsDecree(bool limitColorChangingCards)
        {
            Id = Guid.NewGuid().ToString();
            ImageUrl = $"/images/cards/small/{(int)Color}/{Convert.ToInt32(limitColorChangingCards)}/{(int)Value}.png";
        }
        public string Id { get; }
        public CardColor Color => CardColor.Wild;
        public CardValue Value => CardValue.KingsDecree;
        public string ImageUrl { get; }
        public bool RequirePickColor => true;
        public bool RequireTargetPlayer => false;
        public string Description => "Owner of this card is immune to drawing (If can’t play, skip instead of draw) as long as the number of cards in hand is at least 5. Immune to all stack/draw cards as well as wildcards that require you to draw. Acts as normal wild card if played.  Cards that have a drawback are not affected by this. (devil's deal, summon wildcard, discard wild/color/number, charity)";

    }
}