using System;
using System.Collections.Generic;
using System.Linq;
using Uno.Enums;
using Uno.Models.Entities.Cards.Abstraction;
using Uno.Models.Helpers;

namespace Uno.Models.Entities.Cards.Wild
{
    public class DiscardColor : ICard
    {
        public DiscardColor()
        {
            Id = Guid.NewGuid().ToString();
            Color = CardColor.Wild;
            Value = CardValue.DiscardColor;
            ImageUrl = $"/images/cards/small/{(int)Color}/{(int)Value}.png";
        }
        public string Id { get; set; }
        public CardColor Color { get; set; }
        public CardValue Value { get; set; }
        public string ImageUrl { get; set; }

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {
            var messagesToLog = new List<string>();
            game.Players.ForEach(p =>
                   {
                       var cardsInThatColor = p.Cards.Where(y => y.Color == moveParams.TargetedCardColor).ToList();
                       game.DiscardedPile.AddRange(cardsInThatColor);
                       cardsInThatColor.ForEach(y => p.Cards.Remove(y));

                   });
            Random random = new Random();
            var colorIds = new int[] { 1, 2, 3, 4 };
            int randomColor = colorIds[(random.Next(4))];
            game.LastCardPlayed.Color = (CardColor)randomColor;
            messagesToLog.Add($"{moveParams.PlayerPlayed.User.Name}  played discard color card. All players discarded {moveParams.TargetedCardColor} and a random color has been assigned.");
           return new MoveResult(messagesToLog);
        }
    }
}