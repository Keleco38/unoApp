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

            var automaticallyTriggeredResultMagneticPolarity = _automaticallyTriggeredCardEffectProcessors.First(x => x.CardAffected == CardValue.MagneticPolarity).ProcessCardEffect(game, new AutomaticallyTriggeredParams(moveParams, messageToLog, new List<Player>(), 0));
            messageToLog = automaticallyTriggeredResultMagneticPolarity.MessageToLog;

            var playerLost = callerWon ? moveParams.PlayerTargeted : moveParams.PlayerPlayed;
            var playerWon = callerWon ? moveParams.PlayerPlayed : moveParams.PlayerTargeted;

            var automaticallyTriggeredResultDoubleDraw = _automaticallyTriggeredCardEffectProcessors.First(x => x.CardAffected == CardValue.DoubleDraw).ProcessCardEffect(game,new AutomaticallyTriggeredParams(moveParams, messageToLog, new List<Player>() { playerLost }, 3));
            messageToLog = automaticallyTriggeredResultDoubleDraw.MessageToLog;

            messageToLog += $"{playerWon.User.Name} won! ";

            var automaticallyTriggeredResultDeflect = _automaticallyTriggeredCardEffectProcessors.First(x => x.CardAffected == CardValue.Deflect).ProcessCardEffect(game, new AutomaticallyTriggeredParams(moveParams, messageToLog, null, automaticallyTriggeredResultDoubleDraw.NumberOfCardsToDraw));
            messageToLog = automaticallyTriggeredResultDeflect.MessageToLog;

            messagesToLog.Add(messageToLog);

            return new MoveResult(messagesToLog);
        }
    }
}