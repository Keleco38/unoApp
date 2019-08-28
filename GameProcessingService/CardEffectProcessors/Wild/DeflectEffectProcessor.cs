using System.Collections.Generic;
using EntityObjects;
using GameProcessingService.CoreManagers.GameManagers;
using GameProcessingService.Models;

namespace GameProcessingService.CardEffectProcessors.Wild
{
    public class DeflectEffectProcessor : ICardEffectProcessor
    {
        private readonly IGameManager _gameManager;

        public DeflectEffectProcessor(IGameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {
            var messagesToLog = new List<string>();
            messagesToLog.Add($"{moveParams.PlayerPlayed.User.Name} changed color to {moveParams.TargetedCardColor} (deflect card).");
            return new MoveResult(messagesToLog);
        }
    }
}