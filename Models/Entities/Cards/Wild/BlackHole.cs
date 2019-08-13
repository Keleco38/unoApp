using System;
using System.Collections.Generic;
using System.Linq;
using Uno.Enums;
using Uno.Models;
using unoApp.Models.Abstraction;
using unoApp.Models.Helpers;

namespace unoApp.Models.Entities.Cards.Wild
{
    public class BlackHole : ICard
    {
        public BlackHole()
        {
            Id = Guid.NewGuid().ToString();
            Color = CardColor.Wild;
            Value = CardValue.BlackHole;
            ImageUrl = $"/images/cards/small/{(int)Color}/{(int)Value}.png";
        }
        public string Id { get; set; }
        public CardColor Color { get; set; }
        public CardValue Value { get; set; }
        public string ImageUrl { get; set; }

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {
            var messagesToLog = new List<string>();
            var messageToLog = $"{moveParams.PlayerPlayed.User.Name}  played black hole card. Every player drew new hand. ";
            game.Players.ForEach(p =>
            {
                var keepMyHandCard = p.Cards.FirstOrDefault(y => y.Value == CardValue.KeepMyHand);
                if (keepMyHandCard != null)
                {
                    game.LastCardPlayed = new LastCardPlayed(moveParams.TargetedCardColor, keepMyHandCard.Value, keepMyHandCard.ImageUrl, moveParams.PlayerTargeted.User.Name, true);
                    p.Cards.Remove(keepMyHandCard);
                    game.DiscardedPile.Add(keepMyHandCard);
                    messageToLog += $"{p.User.Name} kept his hand safe. ";
                }
                else
                {
                    var cardCount = p.Cards.Count;
                    game.DiscardedPile.AddRange(p.Cards.ToList());
                    p.Cards.Clear();
                    game.DrawCard(p, cardCount, false);
                }

            });
            messagesToLog.Add(messageToLog);
           return new MoveResult(messagesToLog);
        }
    }
}