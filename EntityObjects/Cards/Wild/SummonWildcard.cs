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
        public string Description => "If the player activates the special effect of the card, they must draw 3 cards (not affected by double draw or king's decree), and can summon any wildcard to their hand if the card exists in the deck or the discarded pile.Player also keeps their turn if they activate the effect. If the player doesn't activate the effect, they must draw 1 card but they will still keep their turn.";

    }
}