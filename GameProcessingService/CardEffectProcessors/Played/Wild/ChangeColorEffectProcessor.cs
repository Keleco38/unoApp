using System.Collections.Generic;
using Common.Enums;
using EntityObjects;
using GameProcessingService.CoreManagers;
using GameProcessingService.Models;

namespace GameProcessingService.CardEffectProcessors.Played.Wild
{
    public class ChangeColorEffectProcessor:IPlayedCardEffectProcessor
    {

        private readonly IGameManager _gameManager; 
 public CardValue CardAffected => CardValue.ChangeColor;

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