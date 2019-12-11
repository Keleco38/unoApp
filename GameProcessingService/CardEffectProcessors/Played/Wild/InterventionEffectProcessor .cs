using System.Collections.Generic;
using System.Linq;
using Common.Enums;
using EntityObjects;
using GameProcessingService.CardEffectProcessors.AutomaticallyTriggered;
using GameProcessingService.CoreManagers;
using GameProcessingService.Models;

namespace GameProcessingService.CardEffectProcessors.Played.Wild
{
    public class InterventionEffectProcessor : IPlayedCardEffectProcessor
    {
        private readonly IGameManager _gameManager;
        private readonly IEnumerable<IAutomaticallyTriggeredCardEffectProcessor> _automaticallyTriggeredCardEffectProcessors;
        public CardValue CardAffected => CardValue.Intervention;

        public InterventionEffectProcessor(IGameManager gameManager, IEnumerable<IAutomaticallyTriggeredCardEffectProcessor> automaticallyTriggeredCardEffectProcessors)
        {
            _gameManager = gameManager;
            _automaticallyTriggeredCardEffectProcessors = automaticallyTriggeredCardEffectProcessors;
        }

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {
            var messagesToLog = new List<string>();
            var messageToLog = $"{moveParams.PlayerPlayed.User.Name} played Intervention. ";

            var playersWithLeastCards = game.Players.Where(x => x.Cards.Count == game.Players.Min(p => p.Cards.Count)).ToList();

            messageToLog += $" Players handcuffed: {string.Join(",", playersWithLeastCards.Select(x => x.User.Name))}";

            foreach (var player in playersWithLeastCards)
            {
                game.HandCuffedPlayers.Add(player);
                var automaticallyTriggeredResultDoubleDraw = _automaticallyTriggeredCardEffectProcessors.First(x => x.CardAffected == CardValue.DoubleDraw).ProcessCardEffect(game, messageToLog, new AutomaticallyTriggeredParams() { DoubleDrawParams = new AutomaticallyTriggeredDoubleDrawParams(player, 1, moveParams.TargetedCardColor) });
                messageToLog = automaticallyTriggeredResultDoubleDraw.MessageToLog;

                var automaticallyTriggeredResultKingsDecree = _automaticallyTriggeredCardEffectProcessors.First(x => x.CardAffected == CardValue.KingsDecree).ProcessCardEffect(game, messageToLog, new AutomaticallyTriggeredParams() { KingsDecreeParams = new AutomaticallyTriggeredKingsDecreeParams() { PlayerAffected = player } });
                messageToLog = automaticallyTriggeredResultKingsDecree.MessageToLog;
                if (!automaticallyTriggeredResultKingsDecree.ActivatedKingsDecree)
                {
                    _gameManager.DrawCard(game, player, automaticallyTriggeredResultDoubleDraw.NumberOfCardsToDraw, false);
                }
            }

            messagesToLog.Add(messageToLog);
            return new MoveResult(messagesToLog);
        }
    }
}