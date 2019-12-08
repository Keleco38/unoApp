using System;
using Common.Enums;
using EntityObjects.Cards.Abstraction;

namespace EntityObjects.Cards.Wild
{
    public class QueensDecree : ICard
    {
        public QueensDecree(bool limitColorChangingCards)
        {
            Id = Guid.NewGuid().ToString();
            ImageUrl = $"/images/cards/small/{(int)Color}/{Convert.ToInt32(limitColorChangingCards)}/{(int)Value}.png";
        }
        public string Id { get; }
        public CardColor Color => CardColor.Wild;
        public CardValue Value => CardValue.QueensDecree;
        public string ImageUrl { get; }
        public bool RequirePickColor => true;
        public bool RequireTargetPlayer => false;
        public string Description => "As long as the owner’s number of cards in hand does not exceed 5, the person to the left and right must draw a card if they have a lesser hand on each turn of the owner. If the game consists of 2 players then only the next player will draw a card.Acts as normal wild card if played. Draw is not affected by double draw card.";

    }
}