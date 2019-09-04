using System.Collections.Generic;
using Common.Enums;
using EntityObjects;
using GameProcessingService.CoreManagers;
using GameProcessingService.Models;

namespace GameProcessingService.CardEffectProcessors.Played.Colored
{
    public class ReverseEffectProcessor : IPlayedCardEffectProcessor
    {
        private readonly IGameManager _gameManager;
        public CardValue CardAffected => CardValue.Reverse;

        public ReverseEffectProcessor(IGameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {
            var messagesToLog = new List<string>();
            var messageToLog=$"{moveParams.PlayerPlayed.User.Name} changed direction. ";
            game.Direction = game.Direction == Direction.Right ? Direction.Left : Direction.Right;

            if (game.GameSetup.ReverseShouldSkipTurnInTwoPlayers && game.Players.Count == 2)
            {
                messageToLog += $"{moveParams.PlayerTargeted.User.Name} was skipped.";
                game.PlayerToPlay = moveParams.PlayerTargeted;
            }

            messagesToLog.Add(messageToLog);
            return new MoveResult(messagesToLog);
        }
    }
}