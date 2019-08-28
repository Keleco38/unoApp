using System.Collections.Generic;
using EntityObjects;
using GameProcessingService.GameManager;
using GameProcessingService.Models;

namespace GameProcessingService.CardEffectProcessors.Wild
{
    public class ChangeColorEffectProcessor:ICardEffectProcessor
    {

        private readonly IGameManager _gameManager;

        public ChangeColorEffectProcessor(IGameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {
            var messagesToLog = new List<string>();
            messagesToLog.Add($"{moveParams.PlayerPlayed.User.Name} changed color to {moveParams.TargetedCardColor}.");
            return new MoveResult(messagesToLog);
        }
    }
}