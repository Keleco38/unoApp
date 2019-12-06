using System;
using System.Collections.Generic;
using Common.Enums;
using EntityObjects;
using GameProcessingService.CoreManagers;
using GameProcessingService.Models;

namespace GameProcessingService.CardEffectProcessors.Played.Wild
{
    public class RussianRouletteEffectProcessor : IPlayedCardEffectProcessor
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
            Random random = new Random();

            while (true)
            {
                int rolledNumber = random.Next(1, 7);
                messageToLog += $" [{moveParams.PlayerTargeted.User.Name}: {rolledNumber}] ";
                if (rolledNumber == 1)
                {
                    _gameManager.DrawCard(game, moveParams.PlayerTargeted, 3, false);
                    messageToLog += $"{moveParams.PlayerTargeted.User.Name} drew 3 cards.";

                    break;
                }
                moveParams.PlayerTargeted = _gameManager.GetNextPlayer(game, moveParams.PlayerTargeted, game.Players);
            }
            messagesToLog.Add(messageToLog);
            return new MoveResult(messagesToLog);
        }
    }
}