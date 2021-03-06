﻿using System;
using System.Collections.Generic;
using Common.Enums;
using EntityObjects.Cards.Abstraction;

namespace EntityObjects.Cards.Wild
{
    public class PromiseKeeper : ICard
    {
        public PromiseKeeper(bool limitColorChangingCards)
        {
            Id = Guid.NewGuid().ToString();
            ImageUrl = $"/images/cards/small/{(int)Color}/{Convert.ToInt32(limitColorChangingCards)}/{(int)Value}.png";
        }
        public string Id { get; }
        public CardColor Color => CardColor.Wild;
        public CardValue Value => CardValue.PromiseKeeper;
        public string ImageUrl { get; }
        public bool RequirePickColor => true;
        public bool RequireTargetPlayer => false;
        public string Description => "Player picks a colored card (can't pick wild) and they promise to play it next turn. If the player plays this card on the next turn, they will discard one card, otherwise they didn't fulfill their promise and they must draw 2 cards.";

    }
}