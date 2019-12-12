using System;
using Common.Enums;
using EntityObjects.Cards.Abstraction;

namespace EntityObjects.Cards.Wild
{
    public class DevilsDeal : ICard
    {
        public DevilsDeal(bool limitColorChangingCards)
        {
            Id = Guid.NewGuid().ToString();
            ImageUrl = $"/images/cards/small/{(int)Color}/{Convert.ToInt32(limitColorChangingCards)}/{(int)Value}.png";
        }
        public string Id { get; }
        public CardColor Color => CardColor.Wild;
        public CardValue Value => CardValue.DevilsDeal;
        public string ImageUrl { get; }
        public bool RequirePickColor => true;
        public bool RequireTargetPlayer => false;
        public string Description => " When the card is played, player can choose to activate the effect or not. If yes, all players with only one card remaining (counted the card is played) must draw a card. The owner must draw 2 cards in return (not affected by double draw or king's decree), and this card will be moved back to their hand. if user decides not to activate effect of the card, they must draw one card (not affected by double draw or king's decree).";

    }
}