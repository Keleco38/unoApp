using System;
using System.Collections.Generic;
using System.Linq;
using Common.Enums;
using EntityObjects;
using GameProcessingService.CardEffectProcessors.AutomaticallyTriggered;
using GameProcessingService.CoreManagers;
using GameProcessingService.Models;

namespace GameProcessingService.CardEffectProcessors.Played.Wild
{
    public class RussianRouletteEffectProcessor : IPlayedCardEffectProcessor
    {
        private readonly IGameManager _gameManager;
        private readonly IEnumerable<IAutomaticallyTriggeredCardEffectProcessor> _automaticallyTriggeredCardEffectProcessors;
        public CardValue CardAffected => CardValue.RussianRoulette;

        public RussianRouletteEffectProcessor(IGameManager gameManager, IEnumerable<IAutomaticallyTriggeredCardEffectProcessor> automaticallyTriggeredCardEffectProcessors)
        {
            _gameManager = gameManager;
            _automaticallyTriggeredCardEffectProcessors = automaticallyTriggeredCardEffectProcessors;
        }

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {
            var messagesToLog = new List<string>();
            var messageToLog = $"{moveParams.PlayerPlayed.User.Name} played Russian Roulette. Every player rolled a dice. ";
            Random random = new Random();

            while (true)
            {
                int rolledNumber = random.Next(1, 7);
                messageToLog += $" [{moveParams.PlayerTargeted.User.Name}: {rolledNumber}] ";
                if (rolledNumber == 1)
                {

                    var automaticallyTriggeredResultDoubleDraw = _automaticallyTriggeredCardEffectProcessors.First(x => x.CardAffected == CardValue.DoubleDraw).ProcessCardEffect(game, new AutomaticallyTriggeredParams(moveParams, messageToLog, new List<Player>() { moveParams.PlayerTargeted }, 3));
                    messageToLog = automaticallyTriggeredResultDoubleDraw.MessageToLog;

                    var automaticallyTriggeredResultDeflect = _automaticallyTriggeredCardEffectProcessors.First(x => x.CardAffected == CardValue.Deflect).ProcessCardEffect(game, new AutomaticallyTriggeredParams(moveParams, messageToLog, null, automaticallyTriggeredResultDoubleDraw.NumberOfCardsToDraw));
                    messageToLog = automaticallyTriggeredResultDeflect.MessageToLog;

                    break;
                }
                moveParams.PlayerTargeted = _gameManager.GetNextPlayer(game, moveParams.PlayerTargeted, game.Players);
            }
            messagesToLog.Add(messageToLog);
            return new MoveResult(messagesToLog);
        }
    }
}