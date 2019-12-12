using System;
using Common.Enums;
using EntityObjects.Cards.Abstraction;

namespace EntityObjects.Cards.Wild
{
    public class Intervention : ICard
    {
        public Intervention(bool limitColorChangingCards)
        {
            Id = Guid.NewGuid().ToString();
            ImageUrl = $"/images/cards/small/{(int)Color}/{Convert.ToInt32(limitColorChangingCards)}/{(int)Value}.png";
            RequirePickColor = !limitColorChangingCards;
        }
        public string Id { get; }
        public CardColor Color => CardColor.Wild;
        public CardValue Value => CardValue.Intervention;
        public string ImageUrl { get; }
        public bool RequirePickColor { get; }
        public bool RequireTargetPlayer => false;
        public string Description => " Players with the least cards in hand (counter after the use), must skip their next turn (get handcuffed) and draw a card. Multiple Interventions can be stacked (one person will skip several turns if they have the least cards).'";

    }
}