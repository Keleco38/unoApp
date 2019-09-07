using System.Collections.Generic;
using Common.Enums;
using EntityObjects;
using GameProcessingService.CoreManagers;
using GameProcessingService.Models;

namespace GameProcessingService.CardEffectProcessors.Played.Colored
{
    public class SkipEffectProcessor : IPlayedCardEffectProcessor
    {
        private readonly IGameManager _gameManager;
        public CardValue CardAffected => CardValue.Skip;

        public SkipEffectProcessor(IGameManager gameManager)
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
            }
            messageToLog += ($"{moveParams.PlayerPlayed.User.Name} played skip turn. {moveParams.PlayerTargeted.User.Name} was skipped");
            game.PlayerToPlay = moveParams.PlayerTargeted;
            messagesToLog.Add(messageToLog);
            return new MoveResult(messagesToLog);

        }
    }
}