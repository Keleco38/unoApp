﻿using System;
using System.Collections.Generic;
using System.Linq;
using Common.Enums;
using Common.Extensions;
using EntityObjects;
using GameProcessingService.CardEffectProcessors.AutomaticallyTriggered;
using GameProcessingService.CoreManagers;
using GameProcessingService.Models;

namespace GameProcessingService.CardEffectProcessors.Played.Wild
{
    public class DuelEffectProcessor : IPlayedCardEffectProcessor
    {
        private readonly IGameManager _gameManager;
        private readonly IEnumerable<IAutomaticallyTriggeredCardEffectProcessor> _automaticallyTriggeredCardEffectProcessors;
        public CardValue CardAffected => CardValue.Duel;

        public DuelEffectProcessor(IGameManager gameManager, IEnumerable<IAutomaticallyTriggeredCardEffectProcessor> automaticallyTriggeredCardEffectProcessors)
        {
            _gameManager = gameManager;
            _automaticallyTriggeredCardEffectProcessors = automaticallyTriggeredCardEffectProcessors;
        }

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {
            var messagesToLog = new List<string>();
            Random random = new Random();
            var numberRolled = random.Next(1, 7);
            var callerWon = moveParams.DuelNumbers.Contains(numberRolled);
            var messageToLog = $"{moveParams.PlayerPlayed.User.Name} targeted {moveParams.PlayerTargeted.User.Name} with Duel. Numbers they picked: {String.Join(' ', moveParams.DuelNumbers)}. Number rolled: {numberRolled}. ";

            var automaticallyTriggeredResultMagneticPolarity = _automaticallyTriggeredCardEffectProcessors.First(x => x.CardAffected == CardValue.MagneticPolarity).ProcessCardEffect(game, messageToLog, new AutomaticallyTriggeredParams() { MagneticPolarityParams = new AutomaticallyTriggeredMagneticPolarityParams(moveParams.TargetedCardColor, moveParams.PlayerPlayed, moveParams.PlayerTargeted) });
            moveParams.PlayerTargeted = automaticallyTriggeredResultMagneticPolarity.MagneticPolaritySelectedPlayer;
            messageToLog = automaticallyTriggeredResultMagneticPolarity.MessageToLog;

            var playerLost = callerWon ? moveParams.PlayerTargeted : moveParams.PlayerPlayed;
            var playerWon = callerWon ? moveParams.PlayerPlayed : moveParams.PlayerTargeted;

            //player won will discard 1 or 2 cards
            var automaticallyTriggeredResultDoubleDraw = _automaticallyTriggeredCardEffectProcessors.First(x => x.CardAffected == CardValue.DoubleDraw).ProcessCardEffect(game, messageToLog, new AutomaticallyTriggeredParams() { DoubleDrawParams = new AutomaticallyTriggeredDoubleDrawParams(playerWon, 1, moveParams.TargetedCardColor) });
            messageToLog = automaticallyTriggeredResultDoubleDraw.MessageToLog;
            messageToLog += $"{playerWon.User.Name} won so they will discard {automaticallyTriggeredResultDoubleDraw.NumberOfCardsToDraw} card(s). ";
            var numberOfCardsToDiscard = playerWon.Cards.Count < automaticallyTriggeredResultDoubleDraw.NumberOfCardsToDraw ? playerWon.Cards.Count : automaticallyTriggeredResultDoubleDraw.NumberOfCardsToDraw;
            var cardsToDiscard = playerWon.Cards.GetAndRemove(0, numberOfCardsToDiscard);
            game.DiscardedPile.AddRange(cardsToDiscard);

            //player lost will draw 3 or 6 cards
            automaticallyTriggeredResultDoubleDraw = _automaticallyTriggeredCardEffectProcessors.First(x => x.CardAffected == CardValue.DoubleDraw).ProcessCardEffect(game, messageToLog, new AutomaticallyTriggeredParams() { DoubleDrawParams = new AutomaticallyTriggeredDoubleDrawParams(playerLost, 3, moveParams.TargetedCardColor) });
            messageToLog = automaticallyTriggeredResultDoubleDraw.MessageToLog;
            messageToLog += $"{playerLost.User.Name} lost so they will draw {automaticallyTriggeredResultDoubleDraw.NumberOfCardsToDraw} card(s). ";

            var automaticallyTriggeredResultKingsDecree = _automaticallyTriggeredCardEffectProcessors.First(x => x.CardAffected == CardValue.KingsDecree).ProcessCardEffect(game, messageToLog, new AutomaticallyTriggeredParams() { KingsDecreeParams = new AutomaticallyTriggeredKingsDecreeParams() { PlayerAffected = playerLost } });
            messageToLog = automaticallyTriggeredResultKingsDecree.MessageToLog;
            if (!automaticallyTriggeredResultKingsDecree.ActivatedKingsDecree)
            {
                _gameManager.DrawCard(game, playerLost, automaticallyTriggeredResultDoubleDraw.NumberOfCardsToDraw, false);
            }

            messagesToLog.Add(messageToLog);

            return new MoveResult(messagesToLog);
        }
    }
}