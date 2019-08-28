using System.Collections.Generic;
using EntityObjects;
using GameProcessingService.GameManager;
using GameProcessingService.Models;

namespace GameProcessingService.CardEffectProcessors.Wild
{
    public class TheLastStandEffectProcessor:ICardEffectProcessor
    {
        private readonly IGameManager _gameManager;

        public TheLastStandEffectProcessor(IGameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {
            var messagesToLog = new List<string>();
            messagesToLog.Add($"{moveParams.PlayerPlayed.User.Name} changed color to {moveParams.TargetedCardColor} (the last stand).");
            return new MoveResult(messagesToLog);
        }
    }
}