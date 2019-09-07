using System.Collections.Generic;
using System.Linq;
using Common.Enums;
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

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams, string messageToLog)
        {
             messageToLog += $"{moveParams.PlayerPlayed.User.Name} played blackjack. They hit {moveParams.BlackjackNumber}. ";
            if (moveParams.BlackjackNumber > 21)
            {
                var automaticallyTriggeredResultDoubleDraw = _automaticallyTriggeredCardEffectProcessors.First(x => x.CardAffected == CardValue.DoubleDraw).ProcessCardEffect(game, messageToLog, new AutomaticallyTriggeredParams() { DoubleDrawParams = new AutomaticallyTriggeredDoubleDrawParams(moveParams.PlayerPlayed, 5, moveParams.TargetedCardColor) });
                messageToLog = automaticallyTriggeredResultDoubleDraw.MessageToLog;

                _gameManager.DrawCard(game, moveParams.PlayerPlayed, automaticallyTriggeredResultDoubleDraw.NumberOfCardsToDraw, false);
                messageToLog += $"They went over 21. They will draw {automaticallyTriggeredResultDoubleDraw.NumberOfCardsToDraw} cards.";
            }
            else if (moveParams.BlackjackNumber == 21)
            {
                var numberToDiscard = moveParams.PlayerPlayed.Cards.Count < 3 ? moveParams.PlayerPlayed.Cards.Count : 3;
                moveParams.PlayerPlayed.Cards.RemoveRange(0, numberToDiscard);
                messageToLog += $"They hit the blackjack. They will discard 3 cards.";

            }
            else if (moveParams.BlackjackNumber < 21 && moveParams.BlackjackNumber > 17)
            {

                var numberToDiscard = moveParams.PlayerPlayed.Cards.Count < 1 ? moveParams.PlayerPlayed.Cards.Count : 1;
                moveParams.PlayerPlayed.Cards.RemoveRange(0, numberToDiscard);
                messageToLog += $"They beat the dealer. They will discard 1 card.";
            }
            else if (moveParams.BlackjackNumber == 17)
            {
                messageToLog += $"It's a draw. Nothing happens. ";
            }
            else
            {
                var numberOfCardsToDraw = 17 - moveParams.BlackjackNumber;
                var automaticallyTriggeredResultDoubleDraw = _automaticallyTriggeredCardEffectProcessors.First(x => x.CardAffected == CardValue.DoubleDraw).ProcessCardEffect(game, messageToLog, new AutomaticallyTriggeredParams() { DoubleDrawParams = new AutomaticallyTriggeredDoubleDrawParams(moveParams.PlayerPlayed, numberOfCardsToDraw, moveParams.TargetedCardColor) });
                messageToLog = automaticallyTriggeredResultDoubleDraw.MessageToLog;

                _gameManager.DrawCard(game, moveParams.PlayerPlayed, automaticallyTriggeredResultDoubleDraw.NumberOfCardsToDraw, false);
                messageToLog += $"They pulled out. They will draw {automaticallyTriggeredResultDoubleDraw.NumberOfCardsToDraw} cards.";
            }
            return new MoveResult(messageToLog);
        }
    }
}