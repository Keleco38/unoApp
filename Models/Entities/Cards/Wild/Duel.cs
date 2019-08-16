using System;
using System.Collections.Generic;
using System.Linq;
using Uno.Enums;
using Uno.Models.Entities.Cards.Abstraction;
using Uno.Models.Helpers;

namespace Uno.Models.Entities.Cards.Wild
{
    public class Duel : ICard
    {
        public Duel()
        {
            Id = Guid.NewGuid().ToString();
            Color = CardColor.Wild;
            Value = CardValue.Duel;
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
            var numberRolled = random.Next(1, 7);
            var maxNumberCalledPicked = moveParams.DuelNumbers.Max();
            var callerWon = moveParams.DuelNumbers.Contains(numberRolled);
            var messageToLog = $"{moveParams.PlayerPlayed.User.Name}  targeted {moveParams.PlayerTargeted.User.Name} with card Duel. Numbers they picked: {String.Join(' ', moveParams.DuelNumbers)}. Number rolled: {numberRolled}. ";

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

            if (callerWon)
            {
                game.DrawCard(moveParams.PlayerTargeted, maxNumberCalledPicked, false);
                messageToLog += $"{moveParams.PlayerPlayed.User.Name} won! {moveParams.PlayerTargeted.User.Name} will draw {maxNumberCalledPicked} cards (max number player selected).";
            }
            else
            {
                game.DrawCard(moveParams.PlayerPlayed, maxNumberCalledPicked, false);
                messageToLog += $"{moveParams.PlayerTargeted.User.Name} won! {moveParams.PlayerPlayed.User.Name} will draw {maxNumberCalledPicked} (max number player selected) cards.";
            }
            messagesToLog.Add(messageToLog);

           return new MoveResult(messagesToLog);
        }
    }
}