using System;
using System.Collections.Generic;
using Common.Enums;
using System.Linq;
using Common.Enums;
using EntityObjects;
using GameProcessingService.CoreManagers;
using GameProcessingService.Models;

namespace GameProcessingService.CardEffectProcessors.Wild
{
    public class RouletteEffectProcessor : ICardEffectProcessor
    {

        private readonly IGameManager _gameManager;
        public CardValue CardAffected => CardValue.Roulette;

        public RouletteEffectProcessor(IGameManager gameManager)
        {
            _gameManager = gameManager;
        }


        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {
            var messagesToLog = new List<string>();
            Random random = new Random();
            var messageToLog = $"{moveParams.PlayerPlayed.User.Name} played Roulette. ";
            var drawOrDiscard = random.Next(2);
            var playerAffected = game.Players[random.Next(game.Players.Count)];
            if (drawOrDiscard == 0)
            {
                //discard   

                var maxNumberToDiscard = playerAffected.Cards.Count < 3 ? playerAffected.Cards.Count + 1 : 3;
                var numberOfCardsToDiscard = random.Next(0, maxNumberToDiscard);
                if (numberOfCardsToDiscard == 0)
                {
                    messageToLog += $"{playerAffected.User.Name} was selected, but They won't discard nor draw any cards. ";
                }
                else
                {
                    playerAffected.Cards.RemoveRange(0, numberOfCardsToDiscard);
                    messageToLog += $"{playerAffected.User.Name} is a lucky winner! They will discard {numberOfCardsToDiscard} cards. ";
                }
            }
            else
            {
                //draw
                var numberOfCardsToDraw = random.Next(1, 5);

                var doubleDrawCard = playerAffected.Cards.FirstOrDefault(c => c.Value == CardValue.DoubleDraw);
                if (doubleDrawCard != null)
                {
                    game.LastCardPlayed = new LastCardPlayed(moveParams.TargetedCardColor, doubleDrawCard.Value, doubleDrawCard.ImageUrl, playerAffected.User.Name, true);
                    playerAffected.Cards.Remove(doubleDrawCard);
                    game.DiscardedPile.Add(doubleDrawCard);
                    numberOfCardsToDraw = numberOfCardsToDraw * 2;
                    messageToLog += $"{playerAffected.User.Name} didn't have any luck! They had double draw. They will draw {numberOfCardsToDraw} cards. ";
                    _gameManager.DrawCard(game, playerAffected, numberOfCardsToDraw, false);
                }
                else
                {
                    messageToLog += $"{playerAffected.User.Name} didn't have any luck! They will draw {numberOfCardsToDraw} cards. ";
                    _gameManager.DrawCard(game, playerAffected, numberOfCardsToDraw, false);
                }

            }

            messagesToLog.Add(messageToLog);
            return new MoveResult(messagesToLog);
        }
    }
}