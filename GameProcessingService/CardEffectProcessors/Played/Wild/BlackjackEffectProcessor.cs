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
    public class BlackjackEffectProcessor : IPlayedCardEffectProcessor
    {
        private readonly IGameManager _gameManager;
        private readonly IEnumerable<IAutomaticallyTriggeredCardEffectProcessor> _automaticallyTriggeredCardEffectProcessors;
        public CardValue CardAffected => CardValue.Blackjack;

        public BlackjackEffectProcessor(IGameManager gameManager, IEnumerable<IAutomaticallyTriggeredCardEffectProcessor> automaticallyTriggeredCardEffectProcessors)
        {
            _gameManager = gameManager;
            _automaticallyTriggeredCardEffectProcessors = automaticallyTriggeredCardEffectProcessors;
        }

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {
            var messagesToLog = new List<string>();
            var messageToLog = $"{moveParams.PlayerPlayed.User.Name} played blackjack. They hit {moveParams.BlackjackNumber}. ";
            if (moveParams.BlackjackNumber > 21)
            {
                var automaticallyTriggeredResultDoubleDraw = _automaticallyTriggeredCardEffectProcessors.First(x => x.CardAffected == CardValue.DoubleDraw).ProcessCardEffect(game, messageToLog, new AutomaticallyTriggeredParams() { DoubleDrawParams = new AutomaticallyTriggeredDoubleDrawParams(moveParams.PlayerPlayed, 5, moveParams.TargetedCardColor) });
                messageToLog = automaticallyTriggeredResultDoubleDraw.MessageToLog;

                messageToLog += $"They went over 21. They will draw {automaticallyTriggeredResultDoubleDraw.NumberOfCardsToDraw} card(s).";

                var automaticallyTriggeredResultKingsDecree = _automaticallyTriggeredCardEffectProcessors.First(x => x.CardAffected == CardValue.KingsDecree).ProcessCardEffect(game, messageToLog, new AutomaticallyTriggeredParams() { KingsDecreeParams = new AutomaticallyTriggeredKingsDecreeParams() { PlayerAffected = moveParams.PlayerPlayed } });
                messageToLog = automaticallyTriggeredResultKingsDecree.MessageToLog;
                if (!automaticallyTriggeredResultKingsDecree.ActivatedKingsDecree)
                {
                    _gameManager.DrawCard(game, moveParams.PlayerPlayed, automaticallyTriggeredResultDoubleDraw.NumberOfCardsToDraw, false);
                }

            }
            else if (moveParams.BlackjackNumber == 21)
            {
                var automaticallyTriggeredResultDoubleDraw = _automaticallyTriggeredCardEffectProcessors.First(x => x.CardAffected == CardValue.DoubleDraw).ProcessCardEffect(game, messageToLog, new AutomaticallyTriggeredParams() { DoubleDrawParams = new AutomaticallyTriggeredDoubleDrawParams(moveParams.PlayerPlayed, 3, moveParams.TargetedCardColor) });
                messageToLog = automaticallyTriggeredResultDoubleDraw.MessageToLog;

                var numberToDiscard = moveParams.PlayerPlayed.Cards.Count < automaticallyTriggeredResultDoubleDraw.NumberOfCardsToDraw ? moveParams.PlayerPlayed.Cards.Count : automaticallyTriggeredResultDoubleDraw.NumberOfCardsToDraw;
                var cardsRemoved = moveParams.PlayerPlayed.Cards.GetAndRemove(0, numberToDiscard);
                game.DiscardedPile.AddRange(cardsRemoved);
                messageToLog += $"They hit the blackjack. They will discard {numberToDiscard} cards.";

            }
            else if (moveParams.BlackjackNumber < 21 && moveParams.BlackjackNumber > 17)
            {
                var automaticallyTriggeredResultDoubleDraw = _automaticallyTriggeredCardEffectProcessors.First(x => x.CardAffected == CardValue.DoubleDraw).ProcessCardEffect(game, messageToLog, new AutomaticallyTriggeredParams() { DoubleDrawParams = new AutomaticallyTriggeredDoubleDrawParams(moveParams.PlayerPlayed, 1, moveParams.TargetedCardColor) });
                messageToLog = automaticallyTriggeredResultDoubleDraw.MessageToLog;

                var numberToDiscard = moveParams.PlayerPlayed.Cards.Count < automaticallyTriggeredResultDoubleDraw.NumberOfCardsToDraw ? moveParams.PlayerPlayed.Cards.Count : automaticallyTriggeredResultDoubleDraw.NumberOfCardsToDraw;
                var cardsRemoved = moveParams.PlayerPlayed.Cards.GetAndRemove(0, numberToDiscard);
                game.DiscardedPile.AddRange(cardsRemoved);

                messageToLog += $"They beat the dealer. They will discard {numberToDiscard} card(s).";
            }
            else if (moveParams.BlackjackNumber == 17)
            {
                messageToLog += $"It's a draw. Nothing happens. ";
            }
            else
            {
                var numberOfCardsToDraw = 17 - moveParams.BlackjackNumber;
                if (numberOfCardsToDraw > 5)
                {
                    numberOfCardsToDraw = 5;
                }
                var automaticallyTriggeredResultDoubleDraw = _automaticallyTriggeredCardEffectProcessors.First(x => x.CardAffected == CardValue.DoubleDraw).ProcessCardEffect(game, messageToLog, new AutomaticallyTriggeredParams() { DoubleDrawParams = new AutomaticallyTriggeredDoubleDrawParams(moveParams.PlayerPlayed, numberOfCardsToDraw, moveParams.TargetedCardColor) });
                messageToLog = automaticallyTriggeredResultDoubleDraw.MessageToLog;

                messageToLog += $"They pulled out. They will draw {automaticallyTriggeredResultDoubleDraw.NumberOfCardsToDraw} card(s).";


                var automaticallyTriggeredResultKingsDecree = _automaticallyTriggeredCardEffectProcessors.First(x => x.CardAffected == CardValue.KingsDecree).ProcessCardEffect(game, messageToLog, new AutomaticallyTriggeredParams() { KingsDecreeParams = new AutomaticallyTriggeredKingsDecreeParams() { PlayerAffected = moveParams.PlayerPlayed } });
                messageToLog = automaticallyTriggeredResultKingsDecree.MessageToLog;
                if (!automaticallyTriggeredResultKingsDecree.ActivatedKingsDecree)
                {
                    _gameManager.DrawCard(game, moveParams.PlayerPlayed, automaticallyTriggeredResultDoubleDraw.NumberOfCardsToDraw, false);
                }

            }
            messagesToLog.Add(messageToLog);
            return new MoveResult(messagesToLog);
        }
    }
}