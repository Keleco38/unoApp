using System.Collections.Generic;
using System.Linq;
using Common.Enums;
using EntityObjects;
using GameProcessingService.CardEffectProcessors.AutomaticallyTriggered;
using GameProcessingService.CoreManagers;
using GameProcessingService.Models;

namespace GameProcessingService.CardEffectProcessors.Played.Wild
{
    public class AssassinateEffectProcessor : IPlayedCardEffectProcessor
    {
        private readonly IGameManager _gameManager;
        private readonly IEnumerable<IAutomaticallyTriggeredCardEffectProcessor> _automaticallyTriggeredCardEffectProcessors;
        public CardValue CardAffected => CardValue.Assassinate;

        public AssassinateEffectProcessor(IGameManager gameManager, IEnumerable<IAutomaticallyTriggeredCardEffectProcessor> automaticallyTriggeredCardEffectProcessors)
        {
            _gameManager = gameManager;
            _automaticallyTriggeredCardEffectProcessors = automaticallyTriggeredCardEffectProcessors;
        }

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {
            var messagesToLog = new List<string>();
            var messageToLog = $"{moveParams.PlayerPlayed.User.Name} played Assassinate. He targeted {moveParams.PlayerTargeted.User.Name} and assassinated card {moveParams.TargetedCardValue.ToString()}. ";


            var automaticallyTriggeredResultMagneticPolarity = _automaticallyTriggeredCardEffectProcessors.First(x => x.CardAffected == CardValue.MagneticPolarity).ProcessCardEffect(game, messageToLog, new AutomaticallyTriggeredParams() { MagneticPolarityParams = new AutomaticallyTriggeredMagneticPolarityParams(moveParams.TargetedCardColor, moveParams.PlayerPlayed, moveParams.PlayerTargeted) });
            moveParams.PlayerTargeted = automaticallyTriggeredResultMagneticPolarity.MagneticPolaritySelectedPlayer;
            messageToLog = automaticallyTriggeredResultMagneticPolarity.MessageToLog;



            var assassinatedCards = moveParams.PlayerTargeted.Cards.Where(x => x.Value == moveParams.TargetedCardValue).ToList();
            if (assassinatedCards.Any())
            {
                foreach (var assassinatedCard in assassinatedCards)
                {
                    moveParams.PlayerTargeted.Cards.Remove(assassinatedCard);
                    game.DiscardedPile.Add(assassinatedCard);
                }

                var automaticallyTriggeredResultDoubleDraw = _automaticallyTriggeredCardEffectProcessors.First(x => x.CardAffected == CardValue.DoubleDraw).ProcessCardEffect(game, messageToLog, new AutomaticallyTriggeredParams() { DoubleDrawParams = new AutomaticallyTriggeredDoubleDrawParams(moveParams.PlayerTargeted, 3, moveParams.TargetedCardColor) });
                messageToLog = automaticallyTriggeredResultDoubleDraw.MessageToLog;

                messageToLog += $"{moveParams.PlayerTargeted.User.Name} discarded {assassinatedCards.Count()} instance(s) of {moveParams.TargetedCardValue.ToString()}. They also drew {automaticallyTriggeredResultDoubleDraw.NumberOfCardsToDraw} cards.";

                var automaticallyTriggeredResultKingsDecree = _automaticallyTriggeredCardEffectProcessors.First(x => x.CardAffected == CardValue.KingsDecree).ProcessCardEffect(game, messageToLog, new AutomaticallyTriggeredParams() { KingsDecreeParams = new AutomaticallyTriggeredKingsDecreeParams() { PlayerAffected = moveParams.PlayerTargeted } });
                messageToLog = automaticallyTriggeredResultKingsDecree.MessageToLog;
                if (!automaticallyTriggeredResultKingsDecree.ActivatedKingsDecree)
                {
                    _gameManager.DrawCard(game, moveParams.PlayerTargeted, automaticallyTriggeredResultDoubleDraw.NumberOfCardsToDraw, false);
                }

            }
            else
            {
                messageToLog += $"{moveParams.PlayerTargeted.User.Name} didn't have that card. Nothing happens.";
            }
            messagesToLog.Add(messageToLog);
            return new MoveResult(messagesToLog);
        }
    }
}