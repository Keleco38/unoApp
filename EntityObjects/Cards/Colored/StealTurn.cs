using System;
using System.Collections.Generic;
using Common.Enums;
using EntityObjects.Cards.Abstraction;

namespace EntityObjects.Cards.Colored
{
    public class StealTurn : ICard
    {
        public StealTurn(CardColor cardColor)
        {
            Id = Guid.NewGuid().ToString();
            Color = cardColor;
        }
        public string Id { get; }
        public CardColor Color { get; }
        public CardValue Value => CardValue.StealTurn;
        public string ImageUrl => $"/images/cards/small/{(int)Color}/{(int)Value}.png";
        public bool RequirePickColor => false;
        public bool RequireTargetPlayer => false;
        public string Description => "Can be played anytime. Steal someone's turn and rotation continues from the user.";

    }
}