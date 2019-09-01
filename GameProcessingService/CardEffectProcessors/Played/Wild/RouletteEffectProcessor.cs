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
    public class RouletteEffectProcessor : IPlayedCardEffectProcessor
    {

        private readonly IGameManager _gameManager;
        private readonly IEnumerable<IAutomaticallyTriggeredCardEffectProcessor> _automaticallyTriggeredCardEffectProcessors;
        public CardValue CardAffected => CardValue.Roulette;

        public RouletteEffectProcessor(IGameManager gameManager, IEnumerable<IAutomaticallyTriggeredCardEffectProcessor> automaticallyTriggeredCardEffectProcessors)
        {
            _gameManager = gameManager;
            _automaticallyTriggeredCardEffectProcessors = automaticallyTriggeredCardEffectProcessors;
        }


        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {
            var messagesToLog = new List<string>();
            Random random = new Random();
            var messageToLog = $"{moveParams.PlayerPlayed.User.Name} played Roulette. ";
            var drawOrDiscard = random.Next(2);
            moveParams.PlayerTargeted = game.Players[random.Next(game.Players.Count)];
            if (drawOrDiscard == 0)
            {
                //discard   
                var maxNumberToDiscard = moveParams.PlayerTargeted.Cards.Count < 3 ? moveParams.PlayerTargeted.Cards.Count + 1 : 3;
                var numberOfCardsToDiscard = random.Next(0, maxNumberToDiscard);
                if (numberOfCardsToDiscard == 0)
                {
                    messageToLog += $"{moveParams.PlayerTargeted.User.Name} was selected, but nothing happened. ";
                }
                else
                {
                    moveParams.PlayerTargeted.Cards.RemoveRange(0, numberOfCardsToDiscard);
                    messageToLog += $"{moveParams.PlayerTargeted.User.Name} is a lucky winner! They will discard {numberOfCardsToDiscard} cards. ";
                }
            }
            else
            {
                //draw   
                messageToLog += $"{moveParams.PlayerTargeted.User.Name} didn't have any luck! ";
                var numberOfCardsToDraw = random.Next(1, 5);

                var automaticallyTriggeredResultDoubleDraw = _automaticallyTriggeredCardEffectProcessors.First(x => x.CardAffected == CardValue.DoubleDraw).ProcessCardEffect(game,new AutomaticallyTriggeredParams(moveParams, messageToLog, new List<Player>() { moveParams.PlayerTargeted }, numberOfCardsToDraw));
                messageToLog = automaticallyTriggeredResultDoubleDraw.MessageToLog;

                var automaticallyTriggeredResultDeflect = _automaticallyTriggeredCardEffectProcessors.First(x => x.CardAffected == CardValue.Deflect).ProcessCardEffect(game, new AutomaticallyTriggeredParams(moveParams, messageToLog, null, automaticallyTriggeredResultDoubleDraw.NumberOfCardsToDraw));
                messageToLog = automaticallyTriggeredResultDeflect.MessageToLog;

            }

            messagesToLog.Add(messageToLog);
            return new MoveResult(messagesToLog);
        }
    }
}