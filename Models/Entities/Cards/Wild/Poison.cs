using System;
using System.Collections.Generic;
using System.Linq;
using Uno.Enums;
using Uno.Models.Entities.Cards.Abstraction;
using Uno.Models.Helpers;

namespace Uno.Models.Entities.Cards.Wild
{
    public class Poison : ICard
    {
        public Poison()
        {
            Id = Guid.NewGuid().ToString();
            Color = CardColor.Wild;
            Value = CardValue.Poison;
            ImageUrl = $"/images/cards/small/{(int)Color}/{(int)Value}.png";
        }
        public string Id { get; set; }
        public CardColor Color { get; set; }
        public CardValue Value { get; set; }
        public string ImageUrl { get; set; }

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {
            var messagesToLog = new List<string>();
            var messageToLog = $"{moveParams.PlayerPlayed.User.Name} changed color to {moveParams.TargetedCardColor} (poison card). ";

            var doubleDrawCard = moveParams.PlayerPlayed.Cards.FirstOrDefault(c => c.Value == CardValue.DoubleDraw);
            if (doubleDrawCard != null)
            {
                game.DrawCard(moveParams.PlayerPlayed, 4, false);
                game.LastCardPlayed = new LastCardPlayed(moveParams.TargetedCardColor, doubleDrawCard.Value, doubleDrawCard.ImageUrl, moveParams.PlayerPlayed.User.Name, true);
                moveParams.PlayerPlayed.Cards.Remove(doubleDrawCard);
                game.DiscardedPile.Add(doubleDrawCard);
                messageToLog += $"{moveParams.PlayerPlayed.User.Name} had double draw, he drew 4 cards. ";
            }
            else
            {
                game.DrawCard(moveParams.PlayerPlayed, 2, false);
                messageToLog += $" {moveParams.PlayerPlayed.User.Name} drew 2 cards. ";
            }
            messagesToLog.Add(messageToLog);
            return new MoveResult(messagesToLog);
        }
    }
}