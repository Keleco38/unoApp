﻿using System;
using System.Collections.Generic;
using Common.Enums;
using EntityObjects;
using GameProcessingService.CoreManagers;
using GameProcessingService.Models;

namespace GameProcessingService.CardEffectProcessors.Played.Wild
{
    public class RandomColorEffectProcessor : IPlayedCardEffectProcessor
    {

        private readonly IGameManager _gameManager;
        public CardValue CardAffected => CardValue.RandomColor;

        public RandomColorEffectProcessor(IGameManager gameManager)
        {
            _gameManager = gameManager;
        }


        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {
            var messagesToLog = new List<string>();
            Random random = new Random();
            var colorIds = new int[] { 1, 2, 3, 4 };
            int randomColor = colorIds[(random.Next(4))];
            game.LastCardPlayed.Color = (CardColor)randomColor;
            messagesToLog.Add($"{moveParams.PlayerPlayed.User.Name} played random color. A random color has been assigned.");
            return new MoveResult(messagesToLog);
        }
    }
}