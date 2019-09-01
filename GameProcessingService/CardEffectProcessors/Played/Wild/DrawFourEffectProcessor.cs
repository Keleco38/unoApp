using System.Collections.Generic;
using System.Linq;
using Common.Enums;
using EntityObjects;
using GameProcessingService.CardEffectProcessors.AutomaticallyTriggered;
using GameProcessingService.CoreManagers;
using GameProcessingService.Models;

namespace GameProcessingService.CardEffectProcessors.Played.Wild
{
    public class DrawFourEffectProcessor : IPlayedCardEffectProcessor
    {
        private readonly IGameManager _gameManager;
        private readonly IEnumerable<IAutomaticallyTriggeredCardEffectProcessor> _automaticallyTriggeredCardEffectProcessors;
        public CardValue CardAffected => CardValue.DrawFour;

        public DrawFourEffectProcessor(IGameManager gameManager, IEnumerable<IAutomaticallyTriggeredCardEffectProcessor> automaticallyTriggeredCardEffectProcessors)
        {
            _gameManager = gameManager;
            _automaticallyTriggeredCardEffectProcessors = automaticallyTriggeredCardEffectProcessors;
        }

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {
            var messagesToLog = new List<string>();
            var messageToLog = $"{moveParams.PlayerPlayed.User.Name} targeted {moveParams.PlayerTargeted.User.Name} with +4.";

            var automaticallyTriggeredResultMagneticPolarity = _automaticallyTriggeredCardEffectProcessors.First(x => x.CardAffected == CardValue.MagneticPolarity).ProcessCardEffect(game, new AutomaticallyTriggeredParams(moveParams, messageToLog, null, 0));
            messageToLog = automaticallyTriggeredResultMagneticPolarity.MessageToLog;

            var automaticallyTriggeredResultDoubleDraw = _automaticallyTriggeredCardEffectProcessors.First(x => x.CardAffected == CardValue.DoubleDraw).ProcessCardEffect(game, new AutomaticallyTriggeredParams(moveParams, messageToLog, new List<Player>() { moveParams.PlayerTargeted }, 4));
            messageToLog = automaticallyTriggeredResultDoubleDraw.MessageToLog;

            var automaticallyTriggeredResultDeflect = _automaticallyTriggeredCardEffectProcessors.First(x => x.CardAffected == CardValue.Deflect).ProcessCardEffect(game, new AutomaticallyTriggeredParams(moveParams, messageToLog, null, automaticallyTriggeredResultDoubleDraw.NumberOfCardsToDraw));
            messageToLog = automaticallyTriggeredResultDeflect.MessageToLog;

            messagesToLog.Add(messageToLog);

            if (game.GameSetup.DrawFourDrawTwoShouldSkipTurn)
            {
                game.PlayerToPlay = moveParams.PlayerTargeted;
            }

            return new MoveResult(messagesToLog);
        }
    }
}