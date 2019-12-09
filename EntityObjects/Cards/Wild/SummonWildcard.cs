using System;
using Common.Enums;
using EntityObjects.Cards.Abstraction;

namespace EntityObjects.Cards.Wild
{
    public class SummonWildcard : ICard
    {
        public SummonWildcard(bool limitColorChangingCards)
        {
            Id = Guid.NewGuid().ToString();
            ImageUrl = $"/images/cards/small/{(int)Color}/{Convert.ToInt32(limitColorChangingCards)}/{(int)Value}.png";
        }
        public string Id { get; }
        public CardColor Color => CardColor.Wild;
        public CardValue Value => CardValue.SummonWildcard;
        public string ImageUrl { get; }
        public bool RequirePickColor => true;
        public bool RequireTargetPlayer => false;
        public string Description => "At the cost of drawing 2 cards, you can summon any wildcard to your hand if it exists in the deck or the discarded pile.  If the card does not exist in the deck or the discarded pile (all instances are in player's hands), then the card is not summoned, but you still pay the price. It cannot be taken from other player's hands. Player keeps his turn after playing this card. Amount of cards required to summon is not affected by double draw.";

    }
}