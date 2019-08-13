using System;
using System.Collections.Generic;
using System.Linq;
using Uno.Enums;
using Uno.Models;
using unoApp.Models.Abstraction;
using unoApp.Models.Helpers;

namespace unoApp.Models.Entities.Cards.Wild
{
    public class Charity : ICard
    {
        public Charity()
        {
            Id = Guid.NewGuid().ToString();
            Color = CardColor.Wild;
            Value = CardValue.Charity;
            ImageUrl = $"/images/cards/small/{(int)Color}/{(int)Value}.png";
        }
        public string Id { get; set; }
        public CardColor Color { get; set; }
        public CardValue Value { get; set; }
        public string ImageUrl { get; set; }

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {
            var messagesToLog = new List<string>();

            var messageToLog = $"{moveParams.PlayerPlayed.User.Name} targeted {moveParams.PlayerTargeted.User.Name} with card Charity. ";
            Player loopingPlayer = moveParams.PlayerPlayed;
            var playerExcludingPlayerPlaying = game.Players.Where(p => p != moveParams.PlayerPlayed).ToList();
            for (int i = 0; i < playerExcludingPlayerPlaying.Count; i++)
            {
                loopingPlayer = game.GetNextPlayer(loopingPlayer, playerExcludingPlayerPlaying);
                var magneticCard = loopingPlayer.Cards.FirstOrDefault(c => c.Value == CardValue.MagneticPolarity);
                if (magneticCard != null)
                {
                    game.LastCardPlayed = new LastCardPlayed(moveParams.TargetedCardColor, magneticCard.Value, magneticCard.ImageUrl, loopingPlayer.User.Name, true);
                    loopingPlayer.Cards.Remove(magneticCard);
                    game.DiscardedPile.Add(magneticCard);
                    messageToLog += ($"{loopingPlayer.User.Name} activated magnetic polarity. He/she was the target instead of {moveParams.PlayerTargeted.User.Name}. ");
                    moveParams.PlayerTargeted = loopingPlayer;
                    break;
                }
            }

            var charityCardsString = string.Empty;
            moveParams.CharityCards.ForEach(x =>
            {
                charityCardsString += x.Color + " " + x.Value + ", ";
                moveParams.PlayerPlayed.Cards.Remove(moveParams.PlayerPlayed.Cards.First(c => c.Value == x.Value && c.Color == x.Color));
                moveParams.PlayerTargeted.Cards.Add(x);
            });
            messageToLog += $" Player gave him two cards: {charityCardsString}";

            messagesToLog.Add(messageToLog);
           return new MoveResult(messagesToLog);
        }
    }
}