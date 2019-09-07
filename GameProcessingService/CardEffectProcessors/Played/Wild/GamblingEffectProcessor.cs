using System.Collections.Generic;
using System.Linq;
using Common.Enums;
using EntityObjects;
using GameProcessingService.CardEffectProcessors.AutomaticallyTriggered;
using GameProcessingService.CoreManagers;
using GameProcessingService.Models;

namespace GameProcessingService.CardEffectProcessors.Played.Wild
{
    public class GamblingEffectProcessor : IPlayedCardEffectProcessor
    {
        private readonly IGameManager _gameManager;
        private readonly IEnumerable<IAutomaticallyTriggeredCardEffectProcessor> _automaticallyTriggeredCardEffectProcessors;
        public CardValue CardAffected => CardValue.Gambling;

        public GamblingEffectProcessor(IGameManager gameManager, IEnumerable<IAutomaticallyTriggeredCardEffectProcessor> automaticallyTriggeredCardEffectProcessors)
        {
            _gameManager = gameManager;
            _automaticallyTriggeredCardEffectProcessors = automaticallyTriggeredCardEffectProcessors;
        }

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams, string messageToLog)
        {
            messageToLog += $"{moveParams.PlayerPlayed.User.Name} targeted {moveParams.PlayerTargeted.User.Name} with Gambling. ";

            var automaticallyTriggeredResultMagneticPolarity = _automaticallyTriggeredCardEffectProcessors.First(x => x.CardAffected == CardValue.MagneticPolarity).ProcessCardEffect(game, messageToLog, new AutomaticallyTriggeredParams() { MagneticPolarityParams = new AutomaticallyTriggeredMagneticPolarityParams(moveParams.TargetedCardColor,moveParams.PlayerPlayed,moveParams.PlayerTargeted) });
            messageToLog = automaticallyTriggeredResultMagneticPolarity.MessageToLog;
            moveParams.PlayerTargeted = automaticallyTriggeredResultMagneticPolarity.MagneticPolaritySelectedPlayer;

            var correctGuess = moveParams.PlayerTargeted.Cards.Count(x => ((int)x.Value < 10)) % 2 == 0 ? "even" : "odd";

            if (correctGuess == moveParams.OddOrEvenGuess)
            {
                messageToLog += $"Player guessed correctly. {moveParams.PlayerTargeted.User.Name} had {correctGuess} number of numbered (0-9) cards. They will discard 1 card";
                if (moveParams.PlayerPlayed.Cards.Count > 0)
                {
                    moveParams.PlayerPlayed.Cards.RemoveRange(0, 1);
                }
            }
            else
            {
                var automaticallyTriggeredResultDoubleDraw = _automaticallyTriggeredCardEffectProcessors.First(x => x.CardAffected == CardValue.DoubleDraw).ProcessCardEffect(game, messageToLog, new AutomaticallyTriggeredParams() { DoubleDrawParams = new AutomaticallyTriggeredDoubleDrawParams(moveParams.PlayerPlayed, 3, moveParams.TargetedCardColor) });
                messageToLog = automaticallyTriggeredResultDoubleDraw.MessageToLog;

                messageToLog += $"Player guessed wrongly. {moveParams.PlayerTargeted.User.Name} had {correctGuess} number of numbered (0-9) cards. They will draw {automaticallyTriggeredResultDoubleDraw.NumberOfCardsToDraw} cards";
                _gameManager.DrawCard(game, moveParams.PlayerPlayed, automaticallyTriggeredResultDoubleDraw.NumberOfCardsToDraw, false);
            }

            return new MoveResult(messageToLog);
        }
    }
}