using System;
using System.Collections.Generic;
using System.Linq;
using Uno.Enums;
using Uno.Models;
using unoApp.Models.Abstraction;
using unoApp.Models.Helpers;

namespace unoApp.Models.Entities.Cards.Wild
{
    public class HandOfGod : ICard
    {
        public HandOfGod()
        {
            Id = Guid.NewGuid().ToString();
            Color = CardColor.Wild;
            Value = CardValue.HandOfGod;
            ImageUrl = $"/images/cards/small/{(int)Color}/{(int)Value}.png";
        }
        public string Id { get; set; }
        public CardColor Color { get; set; }
        public CardValue Value { get; set; }
        public string ImageUrl { get; set; }

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {
            var messagesToLog = new List<string>();
            if (moveParams.PlayerPlayed.Cards.Count > 7)
            {
                messagesToLog.Add($"{moveParams.PlayerPlayed.User.Name}  discarded 4 cards (hand of god). ");
                var cards = moveParams.PlayerPlayed.Cards.Take(4).ToList();
                game.DiscardedPile.AddRange(cards);
                cards.ForEach(y => moveParams.PlayerPlayed.Cards.Remove(y));
            }
            else
            {
                messagesToLog.Add($"{moveParams.PlayerPlayed.User.Name}  didn't discard any cards. He/she had less than 8 cards. (hand of god)");
            }
           return new MoveResult(messagesToLog);
        }
    }
}