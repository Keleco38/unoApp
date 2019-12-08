using System.Collections.Generic;
using System.Linq;
using Common.Enums;
using EntityObjects;
using GameProcessingService.CardEffectProcessors.AutomaticallyTriggered;
using GameProcessingService.CoreManagers;
using GameProcessingService.Models;

namespace GameProcessingService.CardEffectProcessors.Played.Wild
{
    public class SurpriseEffectProcessor : IPlayedCardEffectProcessor
    {
        private readonly IGameManager _gameManager;
        private readonly IEnumerable<IAutomaticallyTriggeredCardEffectProcessor> _automaticallyTriggeredCardEffectProcessors;
        public CardValue CardAffected => CardValue.Surprise;

        public SurpriseEffectProcessor(IGameManager gameManager, IEnumerable<IAutomaticallyTriggeredCardEffectProcessor> automaticallyTriggeredCardEffectProcessors)
        {
            _gameManager = gameManager;
            _automaticallyTriggeredCardEffectProcessors = automaticallyTriggeredCardEffectProcessors;
        }

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {
            var messagesToLog = new List<string>();
            var messageToLog=($"{moveParams.PlayerPlayed.User.Name} played surprise. Every player drew a wild card.");

            game.Players.ForEach(player =>
            {
                var wildCardToAdd = game.Deck.Cards.FirstOrDefault(x => x.Color == CardColor.Wild);
                if (wildCardToAdd == null)
                {
                    wildCardToAdd = game.DiscardedPile.FirstOrDefault(x => x.Color == CardColor.Wild);
                    game.DiscardedPile.Remove(wildCardToAdd);
                }
                else
                {
                    game.Deck.Cards.Remove(wildCardToAdd);
                }


                var automaticallyTriggeredResultKingsDecree = _automaticallyTriggeredCardEffectProcessors.First(x => x.CardAffected == CardValue.KingsDecree).ProcessCardEffect(game, messageToLog, new AutomaticallyTriggeredParams() { KingsDecreeParams = new AutomaticallyTriggeredKingsDecreeParams() { PlayerAffected = player } });
                messageToLog = automaticallyTriggeredResultKingsDecree.MessageToLog;
                if (!automaticallyTriggeredResultKingsDecree.ActivatedKingsDecree)
                {
                    player.Cards.Add(wildCardToAdd);
                }

            });

            messagesToLog.Add(messageToLog);
            return new MoveResult(messagesToLog);
        }
    }
}