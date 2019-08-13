using System;
using System.Collections.Generic;
using System.Linq;
using Uno.Enums;
using Uno.Models.Entities.Cards.Abstraction;
using Uno.Models.Helpers;

namespace Uno.Models.Entities.Cards.Wild
{
    public class FairPlay : ICard
    {
        public FairPlay()
        {
            Id = Guid.NewGuid().ToString();
            Color = CardColor.Wild;
            Value = CardValue.FairPlay;
            ImageUrl = $"/images/cards/small/{(int)Color}/{(int)Value}.png";
        }
        public string Id { get; set; }
        public CardColor Color { get; set; }
        public CardValue Value { get; set; }
        public string ImageUrl { get; set; }

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {
            var messagesToLog = new List<string>();
            var messageToLog = $"{moveParams.PlayerPlayed.User.Name}  targeted {moveParams.PlayerTargeted.User.Name} with card Fair Play. ";

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

            var cardDifference = moveParams.PlayerTargeted.Cards.Count - moveParams.PlayerPlayed.Cards.Count;

            if (cardDifference == 0)
            {
                messageToLog += "No effect. They had the same numer of cards.";
            }
            else if (cardDifference > 0)
            {
                //targeted Player discards
                moveParams.PlayerTargeted.Cards.RemoveRange(0, cardDifference);
                messageToLog += $"{moveParams.PlayerTargeted.User.Name} discarded {cardDifference} cards. He/she had more cards.";
            }
            else
            {
                //targeted Player draws
                var numberInPositiveValue = cardDifference * -1;
                game.DrawCard(moveParams.PlayerTargeted, numberInPositiveValue, false);
                messageToLog += $"{moveParams.PlayerTargeted.User.Name} must draw {numberInPositiveValue} cards. He/she had less cards.";
            }
            messagesToLog.Add(messageToLog);
           return new MoveResult(messagesToLog);
        }
    }
}