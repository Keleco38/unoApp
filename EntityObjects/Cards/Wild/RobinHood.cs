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
        public string Description => "Steals 3 cards from the person with the most cards on the table (counter after this card is played). Keeps 1 for themselves and give 2 to the player with the least cards. If the number of cards stolen is less than 3 then saves 1 for themselves and give 1 away or just give 1 away.";
    }
}