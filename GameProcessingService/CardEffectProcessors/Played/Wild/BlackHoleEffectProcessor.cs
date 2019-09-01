using System.Collections.Generic;
using System.Linq;
using Common.Enums;
using EntityObjects;
using GameProcessingService.CardEffectProcessors.AutomaticallyTriggered;
using GameProcessingService.CoreManagers;
using GameProcessingService.Models;

namespace GameProcessingService.CardEffectProcessors.Played.Wild
{
    public class BlackHoleEffectProcessor : IPlayedCardEffectProcessor
    {
        private readonly IGameManager _gameManager;
        private readonly IEnumerable<IAutomaticallyTriggeredCardEffectProcessor> _automaticallyTriggeredCardEffectProcessors;
        public CardValue CardAffected => CardValue.BlackHole;

        public BlackHoleEffectProcessor(IGameManager gameManager, IEnumerable<IAutomaticallyTriggeredCardEffectProcessor> automaticallyTriggeredCardEffectProcessors)
        {
            _gameManager = gameManager;
            _automaticallyTriggeredCardEffectProcessors = automaticallyTriggeredCardEffectProcessors;
        }

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {
            var messagesToLog = new List<string>();
            var messageToLog = $"{moveParams.PlayerPlayed.User.Name} played black hole. Every player drew new hand. ";

            var automaticallyTriggeredResultKeepMyHand = _automaticallyTriggeredCardEffectProcessors.First(x => x.CardAffected == CardValue.KeepMyHand).ProcessCardEffect(game, new AutomaticallyTriggeredParams(moveParams, messageToLog, game.Players, 0));
            messageToLog = automaticallyTriggeredResultKeepMyHand.MessageToLog;

            automaticallyTriggeredResultKeepMyHand.PlayersWithoutKeepMyHand.ForEach(p =>
                {
                    var cardCount = p.Cards.Count;
                    game.DiscardedPile.AddRange(p.Cards.ToList());
                    p.Cards.Clear();
                    _gameManager.DrawCard(game, p, cardCount, false);
                });

            messagesToLog.Add(messageToLog);
            return new MoveResult(messagesToLog);
        }
    }
}