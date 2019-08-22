using System;
using System.Collections.Generic;
using System.Linq;
using Uno.Enums;
using Uno.Models.Entities.Cards.Abstraction;
using Uno.Models.Helpers;

namespace Uno.Models.Entities.Cards.Colored
{
    public class DrawTwo : ICard
    {
        public DrawTwo(CardColor cardColor)
        {
            Id = Guid.NewGuid().ToString();
            Color = cardColor;
            Value = CardValue.DrawTwo;
            ImageUrl = $"/images/cards/small/{(int)cardColor}/{(int)Value}.png";
        }
        public string Id { get; set; }
        public CardColor Color { get; set; }
        public CardValue Value { get; set; }
        public string ImageUrl { get; set; }

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {
            var messagesToLog = new List<string>();
            var messageToLog = $"{moveParams.PlayerPlayed.User.Name} targeted {moveParams.PlayerTargeted.User.Name} with +2. ";

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

            var numberOfCardsToDraw = 2;

            var doubleDrawCard = moveParams.PlayerTargeted.Cards.FirstOrDefault(c => c.Value == CardValue.DoubleDraw);
            if (doubleDrawCard != null)
            {

                game.LastCardPlayed = new LastCardPlayed(moveParams.TargetedCardColor, doubleDrawCard.Value, doubleDrawCard.ImageUrl, moveParams.PlayerTargeted.User.Name, true);
                moveParams.PlayerTargeted.Cards.Remove(doubleDrawCard);
                game.DiscardedPile.Add(doubleDrawCard);
                numberOfCardsToDraw = numberOfCardsToDraw * 2;
                messageToLog += $"{moveParams.PlayerTargeted.User.Name} doubled the draw effect. ";
            }


            var deflectCard = moveParams.PlayerTargeted.Cards.FirstOrDefault(x => x.Value == CardValue.Deflect);
            if (deflectCard == null)
            {
                messageToLog += $"{moveParams.PlayerTargeted.User.Name} drew {numberOfCardsToDraw} cards.";
                game.DrawCard(moveParams.PlayerTargeted, numberOfCardsToDraw, false);
            }
            else
            {
                game.LastCardPlayed = new LastCardPlayed(moveParams.TargetedCardColor, deflectCard.Value, deflectCard.ImageUrl, moveParams.PlayerTargeted.User.Name, true);
                moveParams.PlayerTargeted.Cards.Remove(deflectCard);
                game.DiscardedPile.Add(deflectCard);
                game.DrawCard(moveParams.PlayerPlayed, numberOfCardsToDraw, false);
                messageToLog += $"{moveParams.PlayerTargeted.User.Name} deflected +2 card. {moveParams.PlayerPlayed.User.Name} must draw {numberOfCardsToDraw} cards.";

            }
            messagesToLog.Add(messageToLog);


           return new MoveResult(messagesToLog);
        }
    }
}