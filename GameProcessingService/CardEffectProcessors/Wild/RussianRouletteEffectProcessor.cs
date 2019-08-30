using System;
using System.Collections.Generic;
using Common.Enums;
using System.Linq;
using EntityObjects;
using GameProcessingService.CoreManagers;
using GameProcessingService.Models;

namespace GameProcessingService.CardEffectProcessors.Wild
{
    public class RussianRouletteEffectProcessor : ICardEffectProcessor
    {
        private readonly IGameManager _gameManager;
        public CardValue CardAffected => CardValue.RussianRoulette;

        public RussianRouletteEffectProcessor(IGameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {
            var messagesToLog = new List<string>();
            var messageToLog = $"{moveParams.PlayerPlayed.User.Name} played Russian Roulette. Every player rolled a dice. ";
            var playerRolling = moveParams.PlayerTargeted;
            Random random = new Random();

            while (true)
            {
                int rolledNumber = random.Next(1, 7);
                messageToLog += $" [{playerRolling.User.Name}: {rolledNumber}] ";
                if (rolledNumber == 1)
                {

                    var doubleDrawCard = playerRolling.Cards.FirstOrDefault(c => c.Value == CardValue.DoubleDraw);
                    if (doubleDrawCard != null)
                    {
                        _gameManager.DrawCard(game, playerRolling, 6, false);

                        game.LastCardPlayed = new LastCardPlayed(moveParams.TargetedCardColor, doubleDrawCard.Value, doubleDrawCard.ImageUrl, playerRolling.User.Name, true);
                        playerRolling.Cards.Remove(doubleDrawCard);
                        game.DiscardedPile.Add(doubleDrawCard);

                        messageToLog += $"{playerRolling.User.Name} had double draw, drew 6 cards. ";
                    }
                    else
                    {
                        messageToLog += $" {playerRolling.User.Name} drew 3 cards. ";
                        _gameManager.DrawCard(game, playerRolling, 3, false);
                    }

                    break;
                }
                playerRolling = _gameManager.GetNextPlayer(game, playerRolling, game.Players);
            }
            messagesToLog.Add(messageToLog);
            return new MoveResult(messagesToLog);
        }
    }
}