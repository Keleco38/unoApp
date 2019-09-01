using System.Collections.Generic;
using System.Linq;
using Common.Enums;
using EntityObjects;
using GameProcessingService.CardEffectProcessors.AutomaticallyTriggered;
using GameProcessingService.CoreManagers;
using GameProcessingService.Models;

namespace GameProcessingService.CardEffectProcessors.Played.Wild
{
    public class JudgementEffectProcessor : IPlayedCardEffectProcessor
    {
        private readonly IGameManager _gameManager;
        private readonly IEnumerable<IAutomaticallyTriggeredCardEffectProcessor> _automaticallyTriggeredCardEffectProcessors;
        public CardValue CardAffected => CardValue.Judgement;

        public JudgementEffectProcessor(IGameManager gameManager, IEnumerable<IAutomaticallyTriggeredCardEffectProcessor> automaticallyTriggeredCardEffectProcessors)
        {
            _gameManager = gameManager;
            _automaticallyTriggeredCardEffectProcessors = automaticallyTriggeredCardEffectProcessors;
        }

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {
            var messagesToLog = new List<string>();
            var messageToLog = $"{moveParams.PlayerPlayed.User.Name} targeted {moveParams.PlayerTargeted.User.Name} with judgement. ";

            var automaticallyTriggeredResultMagneticPolarity = _automaticallyTriggeredCardEffectProcessors.First(x => x.CardAffected == CardValue.MagneticPolarity).ProcessCardEffect(game, new AutomaticallyTriggeredParams(moveParams, messageToLog, new List<Player>(), 0));
            messageToLog = automaticallyTriggeredResultMagneticPolarity.MessageToLog;


            if (moveParams.PlayerTargeted.Cards.Any(x => x.Color == CardColor.Wild))
            {
                var automaticallyTriggeredResultDoubleDraw = _automaticallyTriggeredCardEffectProcessors.First(x => x.CardAffected == CardValue.DoubleDraw).ProcessCardEffect(game, new AutomaticallyTriggeredParams(moveParams, messageToLog, new List<Player>() { moveParams.PlayerTargeted }, 3));
                messageToLog = automaticallyTriggeredResultDoubleDraw.MessageToLog;

                var automaticallyTriggeredResultDeflect = _automaticallyTriggeredCardEffectProcessors.First(x => x.CardAffected == CardValue.Deflect).ProcessCardEffect(game, new AutomaticallyTriggeredParams(moveParams, messageToLog, null, automaticallyTriggeredResultDoubleDraw.NumberOfCardsToDraw));
                messageToLog = automaticallyTriggeredResultDeflect.MessageToLog;
            }
            else
            {
                messageToLog += $"{moveParams.PlayerTargeted.User.Name} didn't draw any cards, They didn't have any wild cards.";
            }
            messagesToLog.Add(messageToLog);
            return new MoveResult(messagesToLog);
        }
    }
}