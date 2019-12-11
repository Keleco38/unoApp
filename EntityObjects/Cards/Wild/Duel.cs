using System;
using System.Collections.Generic;
using System.Linq;
using Common.Enums;
using EntityObjects.Cards.Abstraction;

namespace EntityObjects.Cards.Wild
{
    public class Duel : ICard
    {
        public Duel(bool limitColorChangingCards)
        {
            Id = Guid.NewGuid().ToString();
            ImageUrl = $"/images/cards/small/{(int)Color}/{Convert.ToInt32(limitColorChangingCards)}/{(int)Value}.png";
            RequirePickColor = !limitColorChangingCards;
        }
        public string Id { get; }
        public CardColor Color => CardColor.Wild;
        public CardValue Value => CardValue.Duel;
        public string ImageUrl { get; }
        public bool RequirePickColor { get; }
        public bool RequireTargetPlayer => true;
        public string Description => "Player picks 3 numbers (from 1-6) and picks another player to duel. Dice is rolled afterwards. If the player guessed correctly the number that was rolled, they are a winner and they will discard 1 card while the opponent must draw 3 cards otherwise they lost and they must draw 3 cards while the opponent will discard 1. Cannot be deflected. If the player holds double draw in their hands, they will either draw 6 or discard 2.";
    }
}