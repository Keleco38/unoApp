﻿using System.Collections.Generic;
using EntityObjects;
using GameProcessingService.CoreManagers.GameManagers;
using GameProcessingService.Models;

namespace GameProcessingService.CardEffectProcessors.Colored
{
    public class SkipEffectProcessor:ICardEffectProcessor
    {
        private readonly IGameManager _gameManager;

        public SkipEffectProcessor(IGameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {
            var messagesToLog = new List<string>();
            messagesToLog.Add($"{moveParams.PlayerPlayed.User.Name} played skip turn. {moveParams.PlayerTargeted.User.Name} was skipped");
            game.PlayerToPlay = moveParams.PlayerTargeted;
            return new MoveResult(messagesToLog);
        }
    }
}