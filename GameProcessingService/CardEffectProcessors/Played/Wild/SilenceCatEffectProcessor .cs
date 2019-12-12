using System.Collections.Generic;
using Common.Enums;
using EntityObjects;
using GameProcessingService.CoreManagers;
using GameProcessingService.Models;

namespace GameProcessingService.CardEffectProcessors.Played.Wild
{
    public class SilenceEffectProcessor : IPlayedCardEffectProcessor
    {
        private readonly IGameManager _gameManager;
        public CardValue CardAffected => CardValue.Silence;

        public SilenceEffectProcessor(IGameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {
            var messagesToLog = new List<string>();
            var messageToLog = $"{moveParams.PlayerPlayed.User.Name} played silence. ";

            if (game.SilenceTurnsRemaining <= 0)
            {
                messageToLog += $"For the next 4 turns wild cards will have no effect. ";
                game.SilenceTurnsRemaining = 5;
            }
            else
            {
                messageToLog += $"Game was already silenced so 4 extra turns are added (stacked). ";
                game.SilenceTurnsRemaining += 5;
            }
            messagesToLog.Add(messageToLog);
            return new MoveResult(messagesToLog);
        }
    }
}