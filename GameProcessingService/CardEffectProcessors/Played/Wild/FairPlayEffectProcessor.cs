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
    public class FairPlayEffectProcessor : IPlayedCardEffectProcessor
    {
        private readonly IGameManager _gameManager;
        private readonly IEnumerable<IAutomaticallyTriggeredCardEffectProcessor> _automaticallyTriggeredCardEffectProcessors;
        public CardValue CardAffected => CardValue.FairPlay;

        public FairPlayEffectProcessor(IGameManager gameManager, IEnumerable<IAutomaticallyTriggeredCardEffectProcessor> automaticallyTriggeredCardEffectProcessors)
        {
            _gameManager = gameManager;
            _automaticallyTriggeredCardEffectProcessors = automaticallyTriggeredCardEffectProcessors;
        }

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {
            var messagesToLog = new List<string>();
            var messageToLog = $"{moveParams.PlayerPlayed.User.Name} targeted {moveParams.PlayerTargeted.User.Name} with Fair Play. ";

            if (moveParams.PlayerPlayed.Cards.Count > 9)
            {
                 messageToLog += $"{moveParams.PlayerPlayed.User.Name} had 10 or more cards in hand. Effect is not activated. ";
            }
            else
            {
                var automaticallyTriggeredResultMagneticPolarity = _automaticallyTriggeredCardEffectProcessors.First(x => x.CardAffected == CardValue.MagneticPolarity).ProcessCardEffect(game, messageToLog, new AutomaticallyTriggeredParams() { MagneticPolarityParams = new AutomaticallyTriggeredMagneticPolarityParams(moveParams.TargetedCardColor, moveParams.PlayerPlayed, moveParams.PlayerTargeted) });
                messageToLog = automaticallyTriggeredResultMagneticPolarity.MessageToLog;
                moveParams.PlayerTargeted = automaticallyTriggeredResultMagneticPolarity.MagneticPolaritySelectedPlayer;

                var automaticallyTriggeredResultKeepMyHand = _automaticallyTriggeredCardEffectProcessors.First(x => x.CardAffected == CardValue.KeepMyHand).ProcessCardEffect(game, messageToLog, new AutomaticallyTriggeredParams() { KeepMyHandParams = new AutomaticallyTriggeredKeepMyHandParams(new List<Player>() { moveParams.PlayerTargeted }, moveParams.TargetedCardColor) });
                messageToLog = automaticallyTriggeredResultKeepMyHand.MessageToLog;

                if (automaticallyTriggeredResultKeepMyHand.PlayersWithoutKeepMyHand.Count == 1)
                {
                    var cardDifference = moveParams.PlayerTargeted.Cards.Count - moveParams.PlayerPlayed.Cards.Count;

                    if (cardDifference == 0)
                    {
                        messageToLog += "No effect. They had the same number of cards.";
                    }
                    else if (cardDifference > 0)
                    {
                        var cardsToDiscard = moveParams.PlayerTargeted.Cards.GetAndRemove(0, cardDifference);
                        messageToLog += $"{moveParams.PlayerTargeted.User.Name} discarded {cardDifference} cards. They had more cards.";
                        game.DiscardedPile.AddRange(cardsToDiscard);
                    }
                    else
                    {
                        var numberInPositiveValue = cardDifference * -1;
                        messageToLog += $"{moveParams.PlayerTargeted.User.Name} must draw {numberInPositiveValue} cards. They had less cards.";

                        var automaticallyTriggeredResultKingsDecree = _automaticallyTriggeredCardEffectProcessors.First(x => x.CardAffected == CardValue.KingsDecree).ProcessCardEffect(game, messageToLog, new AutomaticallyTriggeredParams() { KingsDecreeParams = new AutomaticallyTriggeredKingsDecreeParams() { PlayerAffected = moveParams.PlayerTargeted } });
                        messageToLog = automaticallyTriggeredResultKingsDecree.MessageToLog;
                        if (!automaticallyTriggeredResultKingsDecree.ActivatedKingsDecree)
                        {
                            _gameManager.DrawCard(game, moveParams.PlayerTargeted, numberInPositiveValue, false);
                        }
                    }
                }
            }

            messagesToLog.Add(messageToLog);
            return new MoveResult(messagesToLog);
        }
    }
}