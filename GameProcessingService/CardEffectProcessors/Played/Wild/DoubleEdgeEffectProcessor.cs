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

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams, string messageToLog)
        {

            messageToLog += $"{moveParams.PlayerPlayed.User.Name} targeted {moveParams.PlayerTargeted.User.Name} with double edge. ";

            //checking targeted player
            var automaticallyTriggeredResultMagneticPolarity = _automaticallyTriggeredCardEffectProcessors.First(x => x.CardAffected == CardValue.MagneticPolarity).ProcessCardEffect(game, messageToLog, new AutomaticallyTriggeredParams() { MagneticPolarityParams = new AutomaticallyTriggeredMagneticPolarityParams(moveParams.TargetedCardColor,moveParams.PlayerPlayed,moveParams.PlayerTargeted) });
            messageToLog = automaticallyTriggeredResultMagneticPolarity.MessageToLog;
            moveParams.PlayerTargeted = automaticallyTriggeredResultMagneticPolarity.MagneticPolaritySelectedPlayer;
            var automaticallyTriggeredResultDoubleDraw = _automaticallyTriggeredCardEffectProcessors.First(x => x.CardAffected == CardValue.DoubleDraw).ProcessCardEffect(game, messageToLog, new AutomaticallyTriggeredParams() { DoubleDrawParams = new AutomaticallyTriggeredDoubleDrawParams(moveParams.PlayerTargeted, 4, moveParams.TargetedCardColor) });
            messageToLog = automaticallyTriggeredResultDoubleDraw.MessageToLog;
            var automaticallyTriggeredResultDeflect = _automaticallyTriggeredCardEffectProcessors.First(x => x.CardAffected == CardValue.Deflect).ProcessCardEffect(game, messageToLog, new AutomaticallyTriggeredParams() { DeflectParams = new AutomaticallyTriggeredDeflectParams(moveParams.PlayerPlayed, moveParams.PlayerPlayed, automaticallyTriggeredResultDoubleDraw.NumberOfCardsToDraw, moveParams.CardPlayed, moveParams.TargetedCardColor) });
            messageToLog = automaticallyTriggeredResultDeflect.MessageToLog;

            //switch check
            var originallyTargetedPlayer = moveParams.PlayerTargeted;
            moveParams.PlayerTargeted = moveParams.PlayerPlayed;
            moveParams.PlayerPlayed = originallyTargetedPlayer;

            //checking player playing
            var automaticallyTriggeredResultDoubleDraw2 = _automaticallyTriggeredCardEffectProcessors.First(x => x.CardAffected == CardValue.DoubleDraw).ProcessCardEffect(game, messageToLog, new AutomaticallyTriggeredParams() { DoubleDrawParams = new AutomaticallyTriggeredDoubleDrawParams(moveParams.PlayerTargeted, 2, moveParams.TargetedCardColor) });
            messageToLog = automaticallyTriggeredResultDoubleDraw2.MessageToLog;
            var automaticallyTriggeredResultDeflect2 = _automaticallyTriggeredCardEffectProcessors.First(x => x.CardAffected == CardValue.Deflect).ProcessCardEffect(game, messageToLog, new AutomaticallyTriggeredParams() { DeflectParams = new AutomaticallyTriggeredDeflectParams(moveParams.PlayerPlayed, moveParams.PlayerPlayed, automaticallyTriggeredResultDoubleDraw.NumberOfCardsToDraw, moveParams.CardPlayed, moveParams.TargetedCardColor) });
            messageToLog = automaticallyTriggeredResultDeflect2.MessageToLog;

            return new MoveResult(messageToLog);
        }
    }
}