using System.Collections.Generic;
using EntityObjects;
using GameProcessingService.CoreManagers.GameManagers;
using GameProcessingService.Models;

namespace GameProcessingService.CardEffectProcessors.Colored
{
    public class NumberEffectProcessor : ICardEffectProcessor
    {
        private readonly IGameManager _gameManager;

        public NumberEffectProcessor(IGameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {
            var messagesToLog = new List<string>();
            messagesToLog.Add($"{moveParams.PlayerPlayed.User.Name} played {moveParams.CardPlayed.Color.ToString()} {moveParams.CardPlayed.Value.ToString()}.");
            return new MoveResult(messagesToLog);
        }
    }
}