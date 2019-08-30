using System;
using System.Collections.Generic;
using Common.Enums;
using System.Linq;
using EntityObjects;
using GameProcessingService.CoreManagers;
using GameProcessingService.Models;

namespace GameProcessingService.CardEffectProcessors.Wild
{
    public class TricksOfTheTradeEffectProcessor : ICardEffectProcessor
    {
        private readonly IGameManager _gameManager;
        public CardValue CardAffected => CardValue.TricksOfTheTrade;

        public TricksOfTheTradeEffectProcessor(IGameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {
            var messagesToLog = new List<string>();
            Random random = new Random();
            var messageToLog = $"{moveParams.PlayerPlayed.User.Name} targeted {moveParams.PlayerTargeted.User.Name} with Tricks of the trade. ";

            Player loopingPlayer = _gameManager.GetNextPlayer(game, moveParams.PlayerPlayed, game.Players);
            var playerExcludingPlayerPlaying = game.Players.Where(p => p != moveParams.PlayerPlayed).ToList();
            for (int i = 0; i < playerExcludingPlayerPlaying.Count; i++)
            {
                if (i != 0)
                {
                    loopingPlayer = _gameManager.GetNextPlayer(game, loopingPlayer, playerExcludingPlayerPlaying);
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

            var callerNumberToTrade = random.Next(0, moveParams.PlayerPlayed.Cards.Count < 3 ? moveParams.PlayerPlayed.Cards.Count + 1 : 3);
            var targetNumberToTrade = random.Next(0, moveParams.PlayerTargeted.Cards.Count < 3 ? moveParams.PlayerTargeted.Cards.Count + 1 : 3);


            if (callerNumberToTrade == 0)
            {
                messageToLog += $"{moveParams.PlayerPlayed.User.Name} didn't give any cards. ";
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
                messageToLog += $"{moveParams.PlayerTargeted.User.Name} didn't give any cards. ";
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