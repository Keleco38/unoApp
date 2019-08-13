using System;
using System.Collections.Generic;
using System.Linq;
using Uno.Enums;
using Uno.Models;
using unoApp.Models.Abstraction;
using unoApp.Models.Helpers;

namespace unoApp.Models.Entities.Cards.Wild
{
    public class DrawFour : ICard
    {
        public DrawFour()
        {
            Id = Guid.NewGuid().ToString();
            Color = CardColor.Wild;
            Value = CardValue.DrawFour;
            ImageUrl = $"/images/cards/small/{(int)Color}/{(int)Value}.png";
        }
        public string Id { get; set; }
        public CardColor Color { get; set; }
        public CardValue Value { get; set; }
        public string ImageUrl { get; set; }

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {
            var messagesToLog = new List<string>();
            var messageToLog = $"{moveParams.PlayerPlayed.User.Name} targeted {moveParams.PlayerTargeted.User.Name} with +4 card.";

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

            var deflectCard = moveParams.PlayerTargeted.Cards.FirstOrDefault(x => x.Value == CardValue.Deflect);
            if (deflectCard == null)
            {
                messageToLog += $"{moveParams.PlayerTargeted.User.Name} drew 4 cards.";
                game.DrawCard(moveParams.PlayerTargeted, 4, false);
            }
            else
            {
                game.LastCardPlayed = new LastCardPlayed(moveParams.TargetedCardColor, deflectCard.Value, deflectCard.ImageUrl, moveParams.PlayerTargeted.User.Name, true);
                moveParams.PlayerTargeted.Cards.Remove(deflectCard);
                game.DiscardedPile.Add(deflectCard);
                game.DrawCard(moveParams.PlayerPlayed, 4, false);
                messageToLog += $"{moveParams.PlayerTargeted.User.Name} auto deflected +4 card. {moveParams.PlayerPlayed.User.Name} must draw 4 cards.";
            }
            messagesToLog.Add(messageToLog);
           return new MoveResult(messagesToLog);
        }
    }
}