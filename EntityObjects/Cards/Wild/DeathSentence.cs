using System;
using Common.Enums;
using EntityObjects.Cards.Abstraction;

namespace EntityObjects.Cards.Wild
{
    public class DeathSentence : ICard
    {
        public DeathSentence(bool limitColorChangingCards)
        {
            Id = Guid.NewGuid().ToString();
            ImageUrl = $"/images/cards/small/{(int)Color}/{Convert.ToInt32(limitColorChangingCards)}/{(int)Value}.png";
        }
        public string Id { get; }
        public CardColor Color => CardColor.Wild;
        public CardValue Value => CardValue.DeathSentence;
        public string ImageUrl { get; }
        public bool RequirePickColor => true;
        public bool RequireTargetPlayer => false;
        public string Description => "Player can choose to activate the special effect or not. If yes, he can pick a wild card that will be completely removed from the game (only for that round), the card also cannot be summoned by the summon wild card card. Acts as normal wild card otherwise.";

    }
}