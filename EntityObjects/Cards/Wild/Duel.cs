using System;
using System.Collections.Generic;
using System.Linq;
using Common.Enums;
using EntityObjects.Cards.Abstraction;

namespace EntityObjects.Cards.Wild
{
    public class Duel : ICard
    {
        public Duel()
        {
            Id = Guid.NewGuid().ToString();
        }
        public string Id { get; }
        public CardColor Color => CardColor.Wild;
        public CardValue Value => CardValue.Duel;
        public string ImageUrl => $"/images/cards/small/{(int)Color}/{(int)Value}.png";
        public bool RequirePickColor => true;
        public bool RequireTargetPlayer => true;
        public string Description => "Player picks 3 numbers (from 1-6) and picks another player to duel. Dice is rolled afterwards. If the player guessed correctly the number that was rolled, they are a winner and the opponent must draw 3 cards otherwise they lost and they must draw 3 cards.";
    }
}