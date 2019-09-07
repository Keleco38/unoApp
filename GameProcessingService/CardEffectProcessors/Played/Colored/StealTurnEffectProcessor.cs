using System.Collections.Generic;
using Common.Enums;
using EntityObjects;
using GameProcessingService.CoreManagers;
using GameProcessingService.Models;

namespace GameProcessingService.CardEffectProcessors.Played.Colored
{
    public class StealTurnEffectProcessor : IPlayedCardEffectProcessor
    {
        private readonly IGameManager _gameManager;
        public CardValue CardAffected => CardValue.StealTurn;

        public StealTurnEffectProcessor(IGameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams, string messageToLog)
        {
            messageToLog += ($"{moveParams.PlayerPlayed.User.Name} played steal turn. Rotation continues from them.");
            game.PlayerToPlay = moveParams.PlayerPlayed;
            return new MoveResult(messageToLog);
        }
    }
}