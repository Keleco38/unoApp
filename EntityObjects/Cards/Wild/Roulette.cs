using System;
using System.Collections.Generic;
using System.Linq;
using Common.Enums;
using EntityObjects.Cards.Abstraction;

namespace EntityObjects.Cards.Wild
{
    public class Roulette : ICard
    {
        public Roulette()
        {
            Id = Guid.NewGuid().ToString();
        }
        public string Id { get; }
        public CardColor Color => CardColor.Wild;
        public CardValue Value => CardValue.Roulette;
        public string ImageUrl => $"/images/cards/small/{(int)Color}/{(int)Value}.png";
        public bool RequirePickColor => true;
        public bool RequireTargetPlayer => false;
        public string Description => "Randomly selected player will either draw or discard X number of cards, or their hand will remain intact. Maximum number of cards to draw is 4, while the maximum number of cards to discard is 2. Cannot be deflected.";

    }
}