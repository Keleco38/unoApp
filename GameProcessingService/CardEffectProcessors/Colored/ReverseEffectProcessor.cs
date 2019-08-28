using System.Collections.Generic;
using Common.Enums;
using EntityObjects;
using GameProcessingService.GameManager;
using GameProcessingService.Models;

namespace GameProcessingService.CardEffectProcessors.Colored
{
    public class ReverseEffectProcessor : ICardEffectProcessor
    {
        private readonly IGameManager _gameManager;

        public ReverseEffectProcessor(IGameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {
            var messagesToLog = new List<string>();
            messagesToLog.Add($"{moveParams.PlayerPlayed.User.Name} changed direction");
            game.Direction = game.Direction == Direction.Right ? Direction.Left : Direction.Right;
            return new MoveResult(messagesToLog);
        }
    }
}