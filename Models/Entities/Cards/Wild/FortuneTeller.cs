using System;
using System.Collections.Generic;
using System.Linq;
using Uno.Contants;
using Uno.Enums;
using Uno.Models.Entities.Cards.Abstraction;
using Uno.Models.Helpers;

namespace Uno.Models.Entities.Cards.Wild
{
    public class FortuneTeller : ICard
    {
        public FortuneTeller()
        {
            Id = Guid.NewGuid().ToString();
            Color = CardColor.Wild;
            Value = CardValue.FortuneTeller;
            ImageUrl = $"/images/cards/small/{(int)Color}/{(int)Value}.png";
        }
        public string Id { get; set; }
        public CardColor Color { get; set; }
        public CardValue Value { get; set; }
        public string ImageUrl { get; set; }

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {
            var callbackParams = new List<MoveResultCallbackParam>();
            var messagesToLog = new List<string>();
            var messageToLog = $"{moveParams.PlayerPlayed.User.Name} inspected top 5 cards from the deck";
            messagesToLog.Add(messageToLog);

            if (game.PlayerToPlay.Cards.Any())
            {
                callbackParams.Add(new MoveResultCallbackParam(Constants.SHOW_CARDS_CALLBACK_COMMAND, moveParams.PlayerPlayed.User.ConnectionId, game.Deck.Cards.Take(5)));
            }

            return new MoveResult(messagesToLog, callbackParams);
        }
    }
}