
using System;
using Common.Enums;
using EntityObjects.Cards.Abstraction;

namespace EntityObjects.Cards.Wild
{
    public class RobinHood : ICard
    {
        public RobinHood(bool limitColorChangingCards)
        {
            Id = Guid.NewGuid().ToString();
            ImageUrl = $"/images/cards/small/{(int)Color}/{Convert.ToInt32(limitColorChangingCards)}/{(int)Value}.png";
        }
        public string Id { get; }
        public CardColor Color => CardColor.Wild;
        public CardValue Value => CardValue.RobinHood;
        public string ImageUrl { get; }
        public bool RequirePickColor => true;
        public bool RequireTargetPlayer => false;
        public string Description => "Steals 2 cards from the person with the most cards on the table  (if multiple players have the least cards, one is randomly selected, counted after this card is played)  and give them to the player with the least cards. If more than 1 player has max/min number of cards, a random player is selected. Cards taken are revealed.";
    }
}