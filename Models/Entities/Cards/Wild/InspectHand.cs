using System;
using System.Collections.Generic;
using System.Linq;
using Uno.Enums;
using Uno.Models;
using unoApp.Models.Abstraction;
using unoApp.Models.Helpers;

namespace unoApp.Models.Entities.Cards.Wild
{
    public class InspectHand : ICard
    {
        public InspectHand()
        {
            Id = Guid.NewGuid().ToString();
            Color = CardColor.Wild;
            Value = CardValue.InspectHand;
            ImageUrl = $"/images/cards/small/{(int)Color}/{(int)Value}.png";
        }
        public string Id { get; set; }
        public CardColor Color { get; set; }
        public CardValue Value { get; set; }
        public string ImageUrl { get; set; }

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {
            var messagesToLog = new List<string>();
            var moveResultCallbackParams = new List<MoveResultCallbackParam>();

            var messageToLog = ($"{moveParams.PlayerPlayed.User.Name} targeted {moveParams.PlayerTargeted.User.Name} with inspect hand. ");

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

            if (game.PlayerToPlay.Cards.Any())
            {
                var moveResultCallbackParam = new MoveResultCallbackParam("ShowInspectedHand", moveParams.PlayerPlayed.User.ConnectionId, moveParams.PlayerTargeted.Cards);
                moveResultCallbackParams.Add(moveResultCallbackParam);
            }

            messageToLog += ($"{moveParams.PlayerPlayed.User.Name} inspected {moveParams.PlayerTargeted.User.Name}'s hand.");
            messagesToLog.Add(messageToLog);

            return new MoveResult(messagesToLog, moveResultCallbackParams);
        }
    }
}