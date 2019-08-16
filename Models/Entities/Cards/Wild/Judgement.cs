using System;
using System.Collections.Generic;
using System.Linq;
using Uno.Enums;
using Uno.Models.Entities.Cards.Abstraction;
using Uno.Models.Helpers;

namespace Uno.Models.Entities.Cards.Wild
{
    public class Judgement : ICard
    {
        public Judgement()
        {
            Id = Guid.NewGuid().ToString();
            Color = CardColor.Wild;
            Value = CardValue.Judgement;
            ImageUrl = $"/images/cards/small/{(int)Color}/{(int)Value}.png";
        }
        public string Id { get; set; }
        public CardColor Color { get; set; }
        public CardValue Value { get; set; }
        public string ImageUrl { get; set; }

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {
            var messagesToLog = new List<string>();
            var messageToLog = $"{moveParams.PlayerPlayed.User.Name} targeted {moveParams.PlayerTargeted.User.Name} with the judgement card. ";

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
                    messageToLog += ($"{loopingPlayer.User.Name} intercepted attack with magnetic polarity.");
                    moveParams.PlayerTargeted = loopingPlayer;
                    break;
                }
            }

            if (moveParams.PlayerTargeted.Cards.Any(x => x.Color == CardColor.Wild))
            {
                var deflectCard = moveParams.PlayerTargeted.Cards.FirstOrDefault(x => x.Value == CardValue.Deflect);
                if (deflectCard == null)
                {
                    messageToLog += $"{moveParams.PlayerTargeted.User.Name} drew 3 cards. They had a wild card.";
                    game.DrawCard(moveParams.PlayerTargeted, 3, false);
                }
                else
                {
                    game.LastCardPlayed = new LastCardPlayed(moveParams.TargetedCardColor, deflectCard.Value, deflectCard.ImageUrl, moveParams.PlayerTargeted.User.Name, true);
                    moveParams.PlayerTargeted.Cards.Remove(deflectCard);
                    game.DiscardedPile.Add(deflectCard);
                    game.DrawCard(moveParams.PlayerPlayed, 3, false);
                    messageToLog += $"{moveParams.PlayerTargeted.User.Name} auto deflected judgement card. {moveParams.PlayerPlayed.User.Name} must draw 3 cards.";
                }
            }
            else
            {
                messageToLog += $"{moveParams.PlayerTargeted.User.Name} didn't draw any cards, They didn't have any wild cards.";
            }
            messagesToLog.Add(messageToLog);
           return new MoveResult(messagesToLog);
        }
    }
}