using System;
using System.Collections.Generic;
using System.Linq;
using Uno.Enums;
using Uno.Models.Entities.Cards.Abstraction;
using Uno.Models.Helpers;

namespace Uno.Models.Entities.Cards.Wild
{
    public class ParadigmShift : ICard
    {
        public ParadigmShift()
        {
            Id = Guid.NewGuid().ToString();
            Color = CardColor.Wild;
            Value = CardValue.ParadigmShift;
            ImageUrl = $"/images/cards/small/{(int)Color}/{(int)Value}.png";
        }
        public string Id { get; set; }
        public CardColor Color { get; set; }
        public CardValue Value { get; set; }
        public string ImageUrl { get; set; }

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {
            var messagesToLog = new List<string>();
            var messageToLog = $"{moveParams.PlayerPlayed.User.Name} played paradigm shift. Every player exchanged their hand with the next player. ";
            List<ICard> firstCardsBackup = null;

            var playersWithKeepMyHandCard = game.Players.Where(x => x.Cards.FirstOrDefault(y => y.Value == CardValue.KeepMyHand) != null).ToList();
            var playersWithOutKeepMyHandCard = game.Players.Where(x => x.Cards.FirstOrDefault(y => y.Value == CardValue.KeepMyHand) == null).ToList();

            playersWithKeepMyHandCard.ForEach(p =>
            {
                var keepMyHandCard = p.Cards.FirstOrDefault(y => y.Value == CardValue.KeepMyHand);
                game.LastCardPlayed = new LastCardPlayed(moveParams.TargetedCardColor, keepMyHandCard.Value, keepMyHandCard.ImageUrl, p.User.Name, true);
                p.Cards.Remove(keepMyHandCard);
                game.DiscardedPile.Add(keepMyHandCard);
                messageToLog += $"{p.User.Name} kept their hand safe. ";
            });

            Player loopingPlayer = null;

            for (int i = 0; i < playersWithOutKeepMyHandCard.Count; i++)
            {
                if (i == 0)
                {
                    loopingPlayer = playersWithOutKeepMyHandCard[0];
                    firstCardsBackup = loopingPlayer.Cards.ToList();
                }

                if (i != playersWithOutKeepMyHandCard.Count - 1)
                {
                    loopingPlayer.Cards = game.GetNextPlayer(loopingPlayer, playersWithOutKeepMyHandCard).Cards;
                }
                else
                {
                    loopingPlayer.Cards = firstCardsBackup;
                }
                loopingPlayer = game.GetNextPlayer(loopingPlayer, playersWithOutKeepMyHandCard);
            }

            messagesToLog.Add(messageToLog);
           return new MoveResult(messagesToLog);
        }
    }
}