using System;
using System.Collections.Generic;
using System.Linq;
using Uno.Enums;
using Uno.Models.Entities.Cards.Abstraction;
using Uno.Models.Helpers;

namespace Uno.Models.Entities.Cards.Wild
{
    public class TricksOfTheTrade : ICard
    {
        public TricksOfTheTrade()
        {
            Id = Guid.NewGuid().ToString();
            Color = CardColor.Wild;
            Value = CardValue.TricksOfTheTrade;
            ImageUrl = $"/images/cards/small/{(int)Color}/{(int)Value}.png";
        }
        public string Id { get; set; }
        public CardColor Color { get; set; }
        public CardValue Value { get; set; }
        public string ImageUrl { get; set; }

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {
            var messagesToLog = new List<string>();
            Random random = new Random();
            var messageToLog = $"{moveParams.PlayerPlayed.User.Name}  targeted {moveParams.PlayerTargeted.User.Name} with card Tricks of the trade. ";

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

            var callerNumberToTrade = random.Next(0, moveParams.PlayerPlayed.Cards.Count < 4 ? moveParams.PlayerPlayed.Cards.Count + 1 : 4);
            var targetNumberToTrade = random.Next(0, moveParams.PlayerTargeted.Cards.Count < 4 ? moveParams.PlayerTargeted.Cards.Count + 1 : 4);


            if (callerNumberToTrade == 0)
            {
                messageToLog += $"{moveParams.PlayerPlayed.User.Name}  didn't give any cards. ";
            }
            else
            {
                var cardsCallerTraded = moveParams.PlayerPlayed.Cards.GetRange(0, callerNumberToTrade);
                var cardsCallerTradedString = string.Empty;
                cardsCallerTraded.ForEach(x => { cardsCallerTradedString += (x.Color + " " + x.Value + ", "); });
                messageToLog += $"{moveParams.PlayerPlayed.User.Name}  gave {callerNumberToTrade} cards: {cardsCallerTradedString}. ";
                moveParams.PlayerTargeted.Cards.AddRange(cardsCallerTraded);
                cardsCallerTraded.ForEach(x => moveParams.PlayerPlayed.Cards.Remove(x));
            }

            if (targetNumberToTrade == 0)
            {
                messageToLog += $"{moveParams.PlayerTargeted.User.Name}  didn't give any cards. ";
            }
            else
            {
                var cardsTargetTraded = moveParams.PlayerTargeted.Cards.GetRange(0, targetNumberToTrade);
                var cardsTargetTradedString = string.Empty;
                cardsTargetTraded.ForEach(x => { cardsTargetTradedString += (x.Color + " " + x.Value + ", "); });
                messageToLog += $"{moveParams.PlayerTargeted.User.Name} gave {targetNumberToTrade} cards: {cardsTargetTradedString}. ";
                moveParams.PlayerPlayed.Cards.AddRange(cardsTargetTraded);
                cardsTargetTraded.ForEach(x => moveParams.PlayerTargeted.Cards.Remove(x));
            }

            messagesToLog.Add(messageToLog);
           return new MoveResult(messagesToLog);
        }
    }
}