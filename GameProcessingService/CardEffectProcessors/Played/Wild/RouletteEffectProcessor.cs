using System;
using System.Collections.Generic;
using System.Linq;
using Common.Enums;
using Common.Extensions;
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
                var numberOfCardsToDiscard = random.Next(0, 3);
                if (numberOfCardsToDiscard == 0)
                {
                    messageToLog += $"{moveParams.PlayerTargeted.User.Name} was selected, but nothing happened. ";
                }
                else
                {
                    var automaticallyTriggeredResultDoubleDraw = _automaticallyTriggeredCardEffectProcessors.First(x => x.CardAffected == CardValue.DoubleDraw).ProcessCardEffect(game, messageToLog, new AutomaticallyTriggeredParams() { DoubleDrawParams = new AutomaticallyTriggeredDoubleDrawParams(moveParams.PlayerTargeted, numberOfCardsToDiscard, moveParams.TargetedCardColor) });
                    messageToLog = automaticallyTriggeredResultDoubleDraw.MessageToLog;

                    messageToLog += $"{moveParams.PlayerTargeted.User.Name} is a lucky winner! They will discard {automaticallyTriggeredResultDoubleDraw.NumberOfCardsToDraw} card(s). ";

                    numberOfCardsToDiscard = moveParams.PlayerTargeted.Cards.Count < automaticallyTriggeredResultDoubleDraw.NumberOfCardsToDraw ? moveParams.PlayerTargeted.Cards.Count : automaticallyTriggeredResultDoubleDraw.NumberOfCardsToDraw;
                    var cardsToDiscard = moveParams.PlayerTargeted.Cards.GetAndRemove(0, numberOfCardsToDiscard);
                    game.DiscardedPile.AddRange(cardsToDiscard);

                }
            }
            else
            {
                //draw   
                var numberOfCardsToDraw = random.Next(1, 5);
                messageToLog += $"{moveParams.PlayerTargeted.User.Name} didn't have any luck! They must draw {numberOfCardsToDraw} card(s). ";

                var automaticallyTriggeredResultDoubleDraw = _automaticallyTriggeredCardEffectProcessors.First(x => x.CardAffected == CardValue.DoubleDraw).ProcessCardEffect(game, messageToLog, new AutomaticallyTriggeredParams() { DoubleDrawParams = new AutomaticallyTriggeredDoubleDrawParams(moveParams.PlayerTargeted, numberOfCardsToDraw, moveParams.TargetedCardColor) });
                messageToLog = automaticallyTriggeredResultDoubleDraw.MessageToLog;



                var automaticallyTriggeredResultKingsDecree = _automaticallyTriggeredCardEffectProcessors.First(x => x.CardAffected == CardValue.KingsDecree).ProcessCardEffect(game, messageToLog, new AutomaticallyTriggeredParams() { KingsDecreeParams = new AutomaticallyTriggeredKingsDecreeParams() { PlayerAffected = moveParams.PlayerTargeted } });
                messageToLog = automaticallyTriggeredResultKingsDecree.MessageToLog;
                if (!automaticallyTriggeredResultKingsDecree.ActivatedKingsDecree)
                {
                    _gameManager.DrawCard(game, moveParams.PlayerTargeted, automaticallyTriggeredResultDoubleDraw.NumberOfCardsToDraw, false);
                }


            }

            messagesToLog.Add(messageToLog);
            return new MoveResult(messagesToLog);
        }
    }
}