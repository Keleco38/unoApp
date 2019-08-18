using System;
using System.Collections.Generic;
using System.Linq;
using Uno.Enums;
using Uno.Models.Entities.Cards.Abstraction;
using Uno.Models.Helpers;

namespace Uno.Models.Entities.Cards.Wild
{
    public class DoubleEdge : ICard
    {
        public DoubleEdge()
        {
            Id = Guid.NewGuid().ToString();
            Color = CardColor.Wild;
            Value = CardValue.DoubleEdge;
            ImageUrl = $"/images/cards/small/{(int)Color}/{(int)Value}.png";
        }
        public string Id { get; set; }
        public CardColor Color { get; set; }
        public CardValue Value { get; set; }
        public string ImageUrl { get; set; }

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {
            var messagesToLog = new List<string>();
            var messageToLog = $"{moveParams.PlayerPlayed.User.Name} targeted {moveParams.PlayerTargeted.User.Name} with double edge. ";

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

            var numberOfCardsToDrawTarget = 5;
            var numberOfCardsToDrawCaller = 2;
            var doubleDrawCard = moveParams.PlayerTargeted.Cards.FirstOrDefault(c => c.Value == CardValue.DoubleDraw);
            if (doubleDrawCard != null)
            {
                game.LastCardPlayed = new LastCardPlayed(moveParams.TargetedCardColor, doubleDrawCard.Value, doubleDrawCard.ImageUrl, moveParams.PlayerTargeted.User.Name, true);
                moveParams.PlayerTargeted.Cards.Remove(doubleDrawCard);
                game.DiscardedPile.Add(doubleDrawCard);
                numberOfCardsToDrawTarget = numberOfCardsToDrawTarget * 2;
                numberOfCardsToDrawCaller = numberOfCardsToDrawCaller * 2;
                messageToLog += $"{moveParams.PlayerTargeted.User.Name} doubled the draw effect. ";
            }

            var deflectCard = moveParams.PlayerTargeted.Cards.FirstOrDefault(x => x.Value == CardValue.Deflect);
            if (deflectCard == null)
            {
                messageToLog += $"{moveParams.PlayerTargeted.User.Name} drew {numberOfCardsToDrawTarget} cards. ";
                messageToLog += $"{moveParams.PlayerPlayed.User.Name}  drew {numberOfCardsToDrawCaller} cards. ";
                game.DrawCard(moveParams.PlayerTargeted, numberOfCardsToDrawTarget, false);
                game.DrawCard(moveParams.PlayerPlayed, numberOfCardsToDrawCaller, false);
            }
            else
            {
                game.LastCardPlayed = new LastCardPlayed(moveParams.TargetedCardColor, deflectCard.Value, deflectCard.ImageUrl, moveParams.PlayerPlayed.User.Name, true);
                moveParams.PlayerTargeted.Cards.Remove(deflectCard);
                game.DiscardedPile.Add(deflectCard);
                game.DrawCard(moveParams.PlayerPlayed, numberOfCardsToDrawTarget, false);
                game.DrawCard(moveParams.PlayerTargeted, numberOfCardsToDrawCaller, false);
                messageToLog += $"{moveParams.PlayerTargeted.User.Name} deflected double edge card. They will draw {numberOfCardsToDrawCaller} cards and {moveParams.PlayerPlayed.User.Name}  must draw {numberOfCardsToDrawTarget} cards.";
            }
            messagesToLog.Add(messageToLog);
            return new MoveResult(messagesToLog);
        }
    }
}