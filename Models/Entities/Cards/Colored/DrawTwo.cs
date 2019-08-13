using System;
using System.Collections.Generic;
using System.Linq;
using Uno.Enums;
using Uno.Models;
using unoApp.Models.Abstraction;
using unoApp.Models.Helpers;

namespace unoApp.Models.Entities.Cards.Colored
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
            var messageToLog = $"{moveParams.PlayerPlayed.User.Name} targeted {moveParams.PlayerTargeted.User.Name} with +2 card. ";
            var deflectCard = moveParams.PlayerTargeted.Cards.FirstOrDefault(x => x.Value == CardValue.Deflect);
            if (deflectCard == null)
            {
                messageToLog += $"{moveParams.PlayerTargeted.User.Name} drew 2 cards.";
                game.DrawCard(moveParams.PlayerTargeted, 2, false);
            }
            else
            {
                game.LastCardPlayed = new LastCardPlayed(moveParams.TargetedCardColor, deflectCard.Value, deflectCard.ImageUrl, moveParams.PlayerTargeted.User.Name, true);
                moveParams.PlayerTargeted.Cards.Remove(deflectCard);
                game.DiscardedPile.Add(deflectCard);
                game.DrawCard(moveParams.PlayerPlayed, 2, false);
                messageToLog += $"{moveParams.PlayerTargeted.User.Name} auto deflected +2 card. {moveParams.PlayerPlayed.User.Name} must draw 2 cards.";

            }
            messagesToLog.Add(messageToLog);


           return new MoveResult(messagesToLog);
        }
    }
}