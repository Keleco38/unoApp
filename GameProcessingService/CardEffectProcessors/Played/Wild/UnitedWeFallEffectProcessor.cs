using System.Collections.Generic;
using System.Linq;
using Common.Enums;
using EntityObjects;
using GameProcessingService.CardEffectProcessors.AutomaticallyTriggered;
using GameProcessingService.CoreManagers;
using GameProcessingService.Models;

namespace GameProcessingService.CardEffectProcessors.Played.Wild
{
    public class UnitedWeFallEffectProcessor : IPlayedCardEffectProcessor
    {
        private readonly IGameManager _gameManager;
        private readonly IEnumerable<IAutomaticallyTriggeredCardEffectProcessor> _automaticallyTriggeredCardEffectProcessors;
        public CardValue CardAffected => CardValue.UnitedWeFall;

        public UnitedWeFallEffectProcessor(IGameManager gameManager, IEnumerable<IAutomaticallyTriggeredCardEffectProcessor> automaticallyTriggeredCardEffectProcessors)
        {
            _gameManager = gameManager;
            _automaticallyTriggeredCardEffectProcessors = automaticallyTriggeredCardEffectProcessors;
        }

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {
            var messagesToLog = new List<string>();
            var messageToLog = $"{moveParams.PlayerPlayed.User.Name} played united we fall. Every player drew 2 cards. ";


            game.Players.ForEach(p =>
            {
                var automaticallyTriggeredResultDoubleDraw = _automaticallyTriggeredCardEffectProcessors.First(x => x.CardAffected == CardValue.DoubleDraw).ProcessCardEffect(game, messageToLog, new AutomaticallyTriggeredParams() { DoubleDrawParams = new AutomaticallyTriggeredDoubleDrawParams(p, 2, moveParams.TargetedCardColor) });
                messageToLog = automaticallyTriggeredResultDoubleDraw.MessageToLog;

                var automaticallyTriggeredResultKingsDecree = _automaticallyTriggeredCardEffectProcessors.First(x => x.CardAffected == CardValue.KingsDecree).ProcessCardEffect(game, messageToLog, new AutomaticallyTriggeredParams() { KingsDecreeParams = new AutomaticallyTriggeredKingsDecreeParams() { PlayerAffected = p } });
                messageToLog = automaticallyTriggeredResultKingsDecree.MessageToLog;
                if (!automaticallyTriggeredResultKingsDecree.ActivatedKingsDecree)
                {
                    _gameManager.DrawCard(game, p, automaticallyTriggeredResultDoubleDraw.NumberOfCardsToDraw, false);
                }

            });

            messagesToLog.Add(messageToLog);
            return new MoveResult(messagesToLog);
        }
    }
}