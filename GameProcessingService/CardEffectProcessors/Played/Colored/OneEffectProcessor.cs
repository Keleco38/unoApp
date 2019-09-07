using System.Collections.Generic;
using Common.Enums;
using EntityObjects;
using GameProcessingService.CoreManagers;
using GameProcessingService.Models;

namespace GameProcessingService.CardEffectProcessors.Played.Colored
{
    public class OneEffectProcessor : IPlayedCardEffectProcessor
    {
        private readonly IGameManager _gameManager;
        public CardValue CardAffected => CardValue.One;

        public OneEffectProcessor(IGameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams, string messageToLog)
        {
            messageToLog += ($"{moveParams.PlayerPlayed.User.Name} played {moveParams.CardPlayed.Color.ToString()} {moveParams.CardPlayed.Value.ToString()}.");
            return new MoveResult(messageToLog);
        }
    }
}