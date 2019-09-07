using System.Collections.Generic;
using Common.Enums;
using EntityObjects;
using GameProcessingService.CoreManagers;
using GameProcessingService.Models;

namespace GameProcessingService.CardEffectProcessors.Played.Colored
{
    public class SkipEffectProcessor : IPlayedCardEffectProcessor
    {
        private readonly IGameManager _gameManager;
        public CardValue CardAffected => CardValue.Skip;

        public SkipEffectProcessor(IGameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams, string messageToLog)
        {
            messageToLog += ($"{moveParams.PlayerPlayed.User.Name} played skip turn. {moveParams.PlayerTargeted.User.Name} was skipped");
            game.PlayerToPlay = moveParams.PlayerTargeted;
            return new MoveResult(messageToLog);
        }
    }
}