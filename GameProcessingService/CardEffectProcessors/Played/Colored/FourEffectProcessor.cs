using System.Collections.Generic;
using Common.Enums;
using EntityObjects;
using GameProcessingService.CoreManagers;
using GameProcessingService.Models;

namespace GameProcessingService.CardEffectProcessors.Played.Colored
{
    public class FourEffectProcessor : IPlayedCardEffectProcessor
    {
        private readonly IGameManager _gameManager;
        public CardValue CardAffected => CardValue.Four;

        public FourEffectProcessor(IGameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {
            var messagesToLog = new List<string>();
            var messageToLog = string.Empty;
            if (game.GameSetup.MatchingCardStealsTurn && game.PlayerToPlay.User != moveParams.PlayerPlayed.User)
            {
                messageToLog += $"{moveParams.PlayerPlayed.User.Name} stole turn (matching color + value). ";
                game.PlayerToPlay = moveParams.PlayerPlayed;
            }
            messageToLog += $"{moveParams.PlayerPlayed.User.Name} played {moveParams.CardPlayed.Color.ToString()} {moveParams.CardPlayed.Value.ToString()}.";
            messagesToLog.Add(messageToLog);
            return new MoveResult(messagesToLog);
        }
    }
}