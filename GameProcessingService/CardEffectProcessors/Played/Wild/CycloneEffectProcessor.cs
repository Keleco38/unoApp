using System.Collections.Generic;
using System.Linq;
using Common.Enums;
using Common.Extensions;
using EntityObjects;
using EntityObjects.Cards.Abstraction;
using GameProcessingService.CardEffectProcessors.AutomaticallyTriggered;
using GameProcessingService.CoreManagers;
using GameProcessingService.Models;

namespace GameProcessingService.CardEffectProcessors.Played.Wild
{
    public class CycloneEffectProcessor : IPlayedCardEffectProcessor
    {
        private readonly IGameManager _gameManager;
        private readonly IEnumerable<IAutomaticallyTriggeredCardEffectProcessor> _automaticallyTriggeredCardEffectProcessors;
        public CardValue CardAffected => CardValue.Cyclone;

        public CycloneEffectProcessor(IGameManager gameManager, IEnumerable<IAutomaticallyTriggeredCardEffectProcessor> automaticallyTriggeredCardEffectProcessors)
        {
            _gameManager = gameManager;
            _automaticallyTriggeredCardEffectProcessors = automaticallyTriggeredCardEffectProcessors;
        }

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {
            var messagesToLog = new List<string>();
            var messageToLog = $"{moveParams.PlayerPlayed.User.Name} played cyclone. All cards in all hands are shuffled into a pile and then dealt to each person with the same number they had before.  ";

            var automaticallyTriggeredResultKeepMyHand = _automaticallyTriggeredCardEffectProcessors.First(x => x.CardAffected == CardValue.KeepMyHand).ProcessCardEffect(game, messageToLog, new AutomaticallyTriggeredParams() { KeepMyHandParams = new AutomaticallyTriggeredKeepMyHandParams(game.Players, moveParams.TargetedCardColor) });
            messageToLog = automaticallyTriggeredResultKeepMyHand.MessageToLog;

            var allCards = new List<ICard>();
            automaticallyTriggeredResultKeepMyHand.PlayersWithoutKeepMyHand.ForEach(player => allCards.AddRange(player.Cards));
            allCards.Shuffle();

            automaticallyTriggeredResultKeepMyHand.PlayersWithoutKeepMyHand.ForEach(p =>
                {
                    var cardCount = p.Cards.Count;
                    p.Cards.Clear();
                    p.Cards = allCards.GetAndRemove(0, cardCount);
                });

            messagesToLog.Add(messageToLog);
            return new MoveResult(messagesToLog);
        }
    }
}