using System.Collections.Generic;
using System.Linq;
using Common.Enums;
using EntityObjects;
using GameProcessingService.CardEffectProcessors.AutomaticallyTriggered;
using GameProcessingService.CoreManagers;
using GameProcessingService.Models;

namespace GameProcessingService.CardEffectProcessors.Played.Wild
{
    public class PoisonEffectProcessor : IPlayedCardEffectProcessor
    {

        private readonly IGameManager _gameManager;
        private readonly IEnumerable<IAutomaticallyTriggeredCardEffectProcessor> _automaticallyTriggeredCardEffectProcessors;
        public CardValue CardAffected => CardValue.Poison;

        public PoisonEffectProcessor(IGameManager gameManager, IEnumerable<IAutomaticallyTriggeredCardEffectProcessor> automaticallyTriggeredCardEffectProcessors)
        {
            _gameManager = gameManager;
            _automaticallyTriggeredCardEffectProcessors = automaticallyTriggeredCardEffectProcessors;
        }

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {

            var messagesToLog = new List<string>();
            var messageToLog = $"{moveParams.PlayerPlayed.User.Name} changed color to {moveParams.TargetedCardColor} (poison card). ";

            var automaticallyTriggeredResultDoubleDraw = _automaticallyTriggeredCardEffectProcessors.First(x => x.CardAffected == CardValue.DoubleDraw).ProcessCardEffect(game, new AutomaticallyTriggeredParams(moveParams, messageToLog, new List<Player>() { moveParams.PlayerPlayed }, 2));
            messageToLog = automaticallyTriggeredResultDoubleDraw.MessageToLog;
            _gameManager.DrawCard(game, moveParams.PlayerPlayed, automaticallyTriggeredResultDoubleDraw.NumberOfCardsToDraw, false);
            messageToLog += $" {moveParams.PlayerPlayed.User.Name} drew {automaticallyTriggeredResultDoubleDraw.NumberOfCardsToDraw} cards. ";

            messagesToLog.Add(messageToLog);
            return new MoveResult(messagesToLog);
        }
    }
}
