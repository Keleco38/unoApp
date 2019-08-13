using System;
using System.Collections.Generic;
using System.Linq;
using Uno.Enums;
using Uno.Models;
using unoApp.Models.Abstraction;
using unoApp.Models.Helpers;

namespace unoApp.Models.Entities.Cards.Wild
{
    public class SwapHands : ICard
    {
        public SwapHands()
        {
            Id = Guid.NewGuid().ToString();
            Color = CardColor.Wild;
            Value = CardValue.SwapHands;
            ImageUrl = $"/images/cards/small/{(int)Color}/{(int)Value}.png";
        }
        public string Id { get; set; }
        public CardColor Color { get; set; }
        public CardValue Value { get; set; }
        public string ImageUrl { get; set; }

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {
            var messagesToLog = new List<string>();
            var messageToLog = $"{moveParams.PlayerPlayed.User.Name}  targeted {moveParams.PlayerTargeted.User.Name} with swap hands card. ";

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

            var keepMyHandCard = moveParams.PlayerTargeted.Cards.FirstOrDefault(y => y.Value == CardValue.KeepMyHand);
            if (keepMyHandCard != null)
            {
                game.LastCardPlayed = new LastCardPlayed(moveParams.TargetedCardColor, keepMyHandCard.Value, keepMyHandCard.ImageUrl, moveParams.PlayerTargeted.User.Name, true);
                moveParams.PlayerTargeted.Cards.Remove(keepMyHandCard);
                game.DiscardedPile.Add(keepMyHandCard);
                messageToLog += $"{moveParams.PlayerTargeted.User.Name} kept his hand safe. ";
            }
            else
            {
                var playersCards = moveParams.PlayerPlayed.Cards.ToList();
                var targetedPlayerCards = moveParams.PlayerTargeted.Cards.ToList();

                moveParams.PlayerPlayed.Cards = targetedPlayerCards;
                moveParams.PlayerTargeted.Cards = playersCards;
                messageToLog += $"Players exchanged hands.";
            }

            messagesToLog.Add(messageToLog);
           return new MoveResult(messagesToLog);
        }
    }
}