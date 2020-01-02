using System.Collections.Generic;
using System.Linq;
using Common.Enums;
using EntityObjects;
using EntityObjects.Cards.Abstraction;
using GameProcessingService.CardEffectProcessors.AutomaticallyTriggered;
using GameProcessingService.CoreManagers;
using GameProcessingService.Models;

namespace GameProcessingService.CardEffectProcessors.Played.Wild
{
    public class BadgeCollectorEffectProcessor : IPlayedCardEffectProcessor
    {
        private readonly IGameManager _gameManager;
        private readonly IEnumerable<IAutomaticallyTriggeredCardEffectProcessor> _automaticallyTriggeredCardEffectProcessors;
        public CardValue CardAffected => CardValue.BadgeCollector;

        public BadgeCollectorEffectProcessor(IGameManager gameManager, IEnumerable<IAutomaticallyTriggeredCardEffectProcessor> automaticallyTriggeredCardEffectProcessors)
        {
            _gameManager = gameManager;
            _automaticallyTriggeredCardEffectProcessors = automaticallyTriggeredCardEffectProcessors;
        }

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {
            var messagesToLog = new List<string>();
            var messageToLog = $"{moveParams.PlayerPlayed.User.Name} played badge collector. ";


            List<ICard> uniqueCards = new List<ICard>
            {
                moveParams.PlayerPlayed.Cards.FirstOrDefault(x => x.Color == CardColor.Red),
                moveParams.PlayerPlayed.Cards.FirstOrDefault(x => x.Color == CardColor.Green),
                moveParams.PlayerPlayed.Cards.FirstOrDefault(x => x.Color == CardColor.Blue),
                moveParams.PlayerPlayed.Cards.FirstOrDefault(x => x.Color == CardColor.Yellow)
            };


            if (uniqueCards.Contains(null))
            {
                messageToLog += $"They changed color to {moveParams.TargetedCardColor}";
            }
            else
            {
                messageToLog += $"They collected one card of each color. They will discard 4 cards. ";
                moveParams.PlayerPlayed.Cards.RemoveAll(x=>uniqueCards.Contains(x));
            }

            messagesToLog.Add(messageToLog);
            return new MoveResult(messagesToLog);
        }
    }
}