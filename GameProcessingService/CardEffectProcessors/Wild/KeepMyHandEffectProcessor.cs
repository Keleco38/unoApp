using System.Collections.Generic;
using EntityObjects;
using GameProcessingService.CoreManagers.GameManagers;
using GameProcessingService.Models;

namespace GameProcessingService.CardEffectProcessors.Wild
{
    public class KeepMyHandEffectProcessor:ICardEffectProcessor
    {
        private readonly IGameManager _gameManager;

        public KeepMyHandEffectProcessor(IGameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {
            var messagesToLog = new List<string>();
            messagesToLog.Add($"{moveParams.PlayerPlayed.User.Name} changed color to {moveParams.TargetedCardColor} (keep my hand).");
            return new MoveResult(messagesToLog);
        }
    }
}