using System.Collections.Generic;
using Common.Enums;
using EntityObjects;
using GameProcessingService.CoreManagers;
using GameProcessingService.Models;

namespace GameProcessingService.CardEffectProcessors.Played.Wild
{
    public class HandcuffEffectProcessor : IPlayedCardEffectProcessor
    {
        private readonly IGameManager _gameManager;
        public CardValue CardAffected => CardValue.Handcuff;

        public HandcuffEffectProcessor(IGameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {
            var messagesToLog = new List<string>();
            var messageToLog = $"{moveParams.PlayerPlayed.User.Name} played Handcuff and targeted  {moveParams.PlayerTargeted.User.Name}. {moveParams.PlayerTargeted.User.Name} will skip his next turn.";
            game.HandCuffedPlayers.Add(moveParams.PlayerTargeted);
            messagesToLog.Add(messageToLog);
            return new MoveResult(messagesToLog);
        }
    }
}