using System;
using System.Collections.Generic;
using System.Linq;
using Common.Enums;
using EntityObjects.Cards.Abstraction;

namespace EntityObjects.Cards.Wild
{
    public class DoubleEdge : ICard
    {
        public DoubleEdge()
        {
            Id = Guid.NewGuid().ToString();
        }
        public string Id { get; }
        public CardColor Color => CardColor.Wild;
        public CardValue Value => CardValue.DoubleEdge;
        public string ImageUrl => $"/images/cards/small/{(int)Color}/{(int)Value}.png";
        public bool RequirePickColor => true;
        public bool RequireTargetPlayer => true;
        public string Description => "Target a person. They draw 4 cards while the user draws 2 cards. Can be deflected from both players. (max cards one person can draw is 6, if the opposite player deflects, and the player playing doesn't have deflect card)";

    }
}