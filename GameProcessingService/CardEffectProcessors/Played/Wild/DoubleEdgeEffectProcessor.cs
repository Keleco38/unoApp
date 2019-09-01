using System.Collections.Generic;
using System.Linq;
using Common.Enums;
using EntityObjects;
using GameProcessingService.CardEffectProcessors.AutomaticallyTriggered;
using GameProcessingService.CoreManagers;
using GameProcessingService.Models;

namespace GameProcessingService.CardEffectProcessors.Played.Wild
{
    public class DoubleEdgeEffectProcessor : IPlayedCardEffectProcessor
    {

        private readonly IGameManager _gameManager;
        private readonly IEnumerable<IAutomaticallyTriggeredCardEffectProcessor> _automaticallyTriggeredCardEffectProcessors;
        public CardValue CardAffected => CardValue.DoubleEdge;

        public DoubleEdgeEffectProcessor(IGameManager gameManager, IEnumerable<IAutomaticallyTriggeredCardEffectProcessor> automaticallyTriggeredCardEffectProcessors)
        {
            _gameManager = gameManager;
            _automaticallyTriggeredCardEffectProcessors = automaticallyTriggeredCardEffectProcessors;
        }

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {
            var messagesToLog = new List<string>();
            var messageToLog = $"{moveParams.PlayerPlayed.User.Name} targeted {moveParams.PlayerTargeted.User.Name} with double edge. ";

            var automaticallyTriggeredResultMagneticPolarity = _automaticallyTriggeredCardEffectProcessors.First(x => x.CardAffected == CardValue.MagneticPolarity).ProcessCardEffect(game, new AutomaticallyTriggeredParams(moveParams, messageToLog, null, 0));
            messageToLog = automaticallyTriggeredResultMagneticPolarity.MessageToLog;

            var automaticallyTriggeredResultDoubleDraw = _automaticallyTriggeredCardEffectProcessors.First(x => x.CardAffected == CardValue.DoubleDraw).ProcessCardEffect(game, new AutomaticallyTriggeredParams(moveParams, messageToLog, new List<Player>() { moveParams.PlayerTargeted }, 3));
            messageToLog = automaticallyTriggeredResultDoubleDraw.MessageToLog;

            messageToLog += $"Both players drew {automaticallyTriggeredResultDoubleDraw.NumberOfCardsToDraw} cards. ";
            _gameManager.DrawCard(game, moveParams.PlayerTargeted, automaticallyTriggeredResultDoubleDraw.NumberOfCardsToDraw, false);
            _gameManager.DrawCard(game, moveParams.PlayerPlayed, automaticallyTriggeredResultDoubleDraw.NumberOfCardsToDraw, false);

            messagesToLog.Add(messageToLog);
            return new MoveResult(messagesToLog);
        }
    }
}