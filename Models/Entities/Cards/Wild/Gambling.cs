using System;
using System.Collections.Generic;
using System.Linq;
using Uno.Enums;
using Uno.Models.Entities.Cards.Abstraction;
using Uno.Models.Helpers;

namespace Uno.Models.Entities.Cards.Wild
{
    public class Gambling : ICard
    {
        public Gambling()
        {
            Id = Guid.NewGuid().ToString();
            Color = CardColor.Wild;
            Value = CardValue.Gambling;
            ImageUrl = $"/images/cards/small/{(int)Color}/{(int)Value}.png";
        }
        public string Id { get; set; }
        public CardColor Color { get; set; }
        public CardValue Value { get; set; }
        public string ImageUrl { get; set; }

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {
            var messagesToLog = new List<string>();
            var messageToLog = $"{moveParams.PlayerPlayed.User.Name} targeted {moveParams.PlayerTargeted.User.Name} with Gambling. ";

            Player loopingPlayer = game.GetNextPlayer(moveParams.PlayerPlayed, game.Players);
            var playerExcludingPlayerPlaying = game.Players.Where(p => p != moveParams.PlayerPlayed).ToList();
            for (int i = 0; i < playerExcludingPlayerPlaying.Count; i++)
            {
                if (i != 0)
                {
                    loopingPlayer = game.GetNextPlayer(loopingPlayer, playerExcludingPlayerPlaying);
                }

                var magneticCard = loopingPlayer.Cards.FirstOrDefault(c => c.Value == CardValue.MagneticPolarity);
                if (magneticCard != null)
                {
                    game.LastCardPlayed = new LastCardPlayed(moveParams.TargetedCardColor, magneticCard.Value, magneticCard.ImageUrl, loopingPlayer.User.Name, true);
                    loopingPlayer.Cards.Remove(magneticCard);
                    game.DiscardedPile.Add(magneticCard);
                    messageToLog += ($"{loopingPlayer.User.Name} intercepted attack with magnetic polarity. ");
                    moveParams.PlayerTargeted = loopingPlayer;
                    break;
                }
            }

            var isEven = moveParams.PlayerTargeted.Cards.Count(x => ((int)x.Value < 10)) % 2 == 0;

            if ((isEven && moveParams.OddOrEvenGuess == "even") || (!isEven && moveParams.OddOrEvenGuess == "odd"))
            {
                messageToLog += $"Player guessed correctly. {moveParams.PlayerPlayed.User.Name} had {moveParams.OddOrEvenGuess} number of cards. They will discard 1 card";
                if (moveParams.PlayerPlayed.Cards.Count > 1)
                {
                    moveParams.PlayerPlayed.Cards.RemoveRange(0, 1);
                }
            }
            else
            {
                var numberOfCardsToDraw = 3;

                var doubleDrawCard = moveParams.PlayerPlayed.Cards.FirstOrDefault(c => c.Value == CardValue.DoubleDraw);
                if (doubleDrawCard != null)
                {
                    game.LastCardPlayed = new LastCardPlayed(moveParams.TargetedCardColor, doubleDrawCard.Value, doubleDrawCard.ImageUrl, moveParams.PlayerPlayed.User.Name, true);
                    moveParams.PlayerPlayed.Cards.Remove(doubleDrawCard);
                    game.DiscardedPile.Add(doubleDrawCard);
                    numberOfCardsToDraw = numberOfCardsToDraw * 2;
                    messageToLog += $"{moveParams.PlayerPlayed.User.Name} doubled the draw effect. ";
                }

                messageToLog += $"Player guessed wrongly. {moveParams.PlayerTargeted.User.Name} had {moveParams.OddOrEvenGuess} number of cards. They will draw {numberOfCardsToDraw} cards";
                game.DrawCard(moveParams.PlayerPlayed, numberOfCardsToDraw, false);
                //double draw
            }

            messagesToLog.Add(messageToLog);
            return new MoveResult(messagesToLog);
        }
    }
}