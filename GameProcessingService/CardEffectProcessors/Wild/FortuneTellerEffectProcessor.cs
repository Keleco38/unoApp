﻿using System.Collections.Generic;
using System.Linq;
using Common.Contants;
using EntityObjects;
using GameProcessingService.CoreManagers.GameManagers;
using GameProcessingService.Models;

namespace GameProcessingService.CardEffectProcessors.Wild
{
    public class FortuneTellerEffectProcessor:ICardEffectProcessor
    {
        private readonly IGameManager _gameManager;

        public FortuneTellerEffectProcessor(IGameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {
            var callbackParams = new List<MoveResultCallbackParam>();
            var messagesToLog = new List<string>();
            var messageToLog = $"{moveParams.PlayerPlayed.User.Name} inspected top 5 cards from the deck";
            messagesToLog.Add(messageToLog);

            if (game.PlayerToPlay.Cards.Any())
            {
                callbackParams.Add(new MoveResultCallbackParam(Constants.SHOW_CARDS_CALLBACK_COMMAND, moveParams.PlayerPlayed.User.ConnectionId, game.Deck.Cards.Take(5)));
            }

            return new MoveResult(messagesToLog, callbackParams);
        }
    }
}