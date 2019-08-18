using System;
using System.Collections.Generic;
using System.Linq;
using Uno.Enums;
using Uno.Models.Entities.Cards.Abstraction;
using Uno.Models.Helpers;

namespace Uno.Models.Entities.Cards.Wild
{
    public class UnitedWeFall : ICard
    {
        public UnitedWeFall()
        {
            Id = Guid.NewGuid().ToString();
            Color = CardColor.Wild;
            Value = CardValue.UnitedWeFall;
            ImageUrl = $"/images/cards/small/{(int)Color}/{(int)Value}.png";
        }
        public string Id { get; set; }
        public CardColor Color { get; set; }
        public CardValue Value { get; set; }
        public string ImageUrl { get; set; }

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {
            var messagesToLog = new List<string>();
            var messageToLog = $"{moveParams.PlayerPlayed.User.Name}  played united we fall. Every player drew 2 cards. ";

            game.Players.ForEach(x =>
            {
                var doubleDrawCard = x.Cards.FirstOrDefault(c => c.Value == CardValue.DoubleDraw);
                if (doubleDrawCard != null)
                {
                    game.DrawCard(x, 4, false);

                    game.LastCardPlayed = new LastCardPlayed(moveParams.TargetedCardColor, doubleDrawCard.Value, doubleDrawCard.ImageUrl, x.User.Name, true);
                    x.Cards.Remove(doubleDrawCard);
                    game.DiscardedPile.Add(doubleDrawCard);

                    messageToLog += $"{x.User.Name}  drew double.";
                }
                else
                {
                    game.DrawCard(x, 2, false);
                }
            });
            messagesToLog.Add(messageToLog);
            return new MoveResult(messagesToLog);
        }
    }
}