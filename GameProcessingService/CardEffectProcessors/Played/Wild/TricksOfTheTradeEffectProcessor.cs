using System;
using System.Collections.Generic;
using System.Linq;
using Common.Enums;
using EntityObjects;
using GameProcessingService.CardEffectProcessors.AutomaticallyTriggered;
using GameProcessingService.CoreManagers;
using GameProcessingService.Models;

namespace GameProcessingService.CardEffectProcessors.Played.Wild
{
    public class TricksOfTheTradeEffectProcessor : IPlayedCardEffectProcessor
    {
        private readonly IGameManager _gameManager;
        private readonly IEnumerable<IAutomaticallyTriggeredCardEffectProcessor> _automaticallyTriggeredCardEffectProcessors;
        public CardValue CardAffected => CardValue.TricksOfTheTrade;

        public TricksOfTheTradeEffectProcessor(IGameManager gameManager, IEnumerable<IAutomaticallyTriggeredCardEffectProcessor> automaticallyTriggeredCardEffectProcessors)
        {
            _gameManager = gameManager;
            _automaticallyTriggeredCardEffectProcessors = automaticallyTriggeredCardEffectProcessors;
        }

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {
            var messagesToLog = new List<string>();
            Random random = new Random();
            var messageToLog = $"{moveParams.PlayerPlayed.User.Name} targeted {moveParams.PlayerTargeted.User.Name} with Tricks of the trade. ";

            var automaticallyTriggeredResultMagneticPolarity = _automaticallyTriggeredCardEffectProcessors.First(x => x.CardAffected == CardValue.MagneticPolarity).ProcessCardEffect(game, new AutomaticallyTriggeredParams(moveParams, messageToLog, new List<Player>(), 0));
            messageToLog = automaticallyTriggeredResultMagneticPolarity.MessageToLog;

            var callerNumberToTrade = random.Next(0, moveParams.PlayerPlayed.Cards.Count < 3 ? moveParams.PlayerPlayed.Cards.Count + 1 : 3);
            var targetNumberToTrade = random.Next(0, moveParams.PlayerTargeted.Cards.Count < 3 ? moveParams.PlayerTargeted.Cards.Count + 1 : 3);


            if (callerNumberToTrade == 0)
            {
                messageToLog += $"{moveParams.PlayerPlayed.User.Name} didn't give any cards. ";
            }
            else
            {
                var cardsCallerTraded = moveParams.PlayerPlayed.Cards.GetRange(0, callerNumberToTrade);
                var cardsCallerTradedString = string.Empty;
                cardsCallerTraded.ForEach(x => { cardsCallerTradedString += (x.Color + " " + x.Value + ", "); });
                messageToLog += $"{moveParams.PlayerPlayed.User.Name}  gave {callerNumberToTrade} cards: {cardsCallerTradedString}. ";
                moveParams.PlayerTargeted.Cards.AddRange(cardsCallerTraded);
                cardsCallerTraded.ForEach(x => moveParams.PlayerPlayed.Cards.Remove(x));
            }

            if (targetNumberToTrade == 0)
            {
                messageToLog += $"{moveParams.PlayerTargeted.User.Name} didn't give any cards. ";
            }
            else
            {
                var cardsTargetTraded = moveParams.PlayerTargeted.Cards.GetRange(0, targetNumberToTrade);
                var cardsTargetTradedString = string.Empty;
                cardsTargetTraded.ForEach(x => { cardsTargetTradedString += (x.Color + " " + x.Value + ", "); });
                messageToLog += $"{moveParams.PlayerTargeted.User.Name} gave {targetNumberToTrade} cards: {cardsTargetTradedString}. ";
                moveParams.PlayerPlayed.Cards.AddRange(cardsTargetTraded);
                cardsTargetTraded.ForEach(x => moveParams.PlayerTargeted.Cards.Remove(x));
            }

            messagesToLog.Add(messageToLog);
            return new MoveResult(messagesToLog);
        }

    }
}