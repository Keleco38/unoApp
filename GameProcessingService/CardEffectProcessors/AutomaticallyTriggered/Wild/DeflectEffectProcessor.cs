using System.Linq;
using Common.Enums;
using EntityObjects;
using GameProcessingService.CoreManagers;
using GameProcessingService.Models;

namespace GameProcessingService.CardEffectProcessors.AutomaticallyTriggered.Wild
{
    public class DeflectEffectProcessor : IAutomaticallyTriggeredCardEffectProcessor
    {
        private readonly IGameManager _gameManager;
        public CardValue CardAffected => CardValue.Deflect;

        public DeflectEffectProcessor(IGameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public AutomaticallyTriggeredResult ProcessCardEffect(Game game, string messageToLog, AutomaticallyTriggeredParams autoParams)
        {
            if (autoParams.DeflectParams.PlayerPlayed == autoParams.DeflectParams.PlayerTargeted)
            {
                messageToLog += $"{autoParams.DeflectParams.PlayerTargeted.User.Name} drew {autoParams.DeflectParams.NumberOfCardsToDraw} cards. ";

                var kingsDecreeResult = BlockedByKingsDecree(messageToLog, autoParams.DeflectParams.PlayerTargeted,game);
                messageToLog = kingsDecreeResult.MessageToLog;
                if (!kingsDecreeResult.ActivatedKingsDecree)
                {
                    _gameManager.DrawCard(game, autoParams.DeflectParams.PlayerTargeted, autoParams.DeflectParams.NumberOfCardsToDraw, false);
                }
            }
            else
            {
                var deflectCard = autoParams.DeflectParams.PlayerTargeted.Cards.FirstOrDefault(x => x.Value == CardValue.Deflect);
                if (deflectCard == null)
                {
                    messageToLog += $"{autoParams.DeflectParams.PlayerTargeted.User.Name} drew {autoParams.DeflectParams.NumberOfCardsToDraw} cards. ";

                    var kingsDecreeResult = BlockedByKingsDecree(messageToLog, autoParams.DeflectParams.PlayerTargeted,game);
                    messageToLog = kingsDecreeResult.MessageToLog;
                    if (!kingsDecreeResult.ActivatedKingsDecree)
                    {
                        _gameManager.DrawCard(game, autoParams.DeflectParams.PlayerTargeted, autoParams.DeflectParams.NumberOfCardsToDraw, false);
                    }
                }
                else
                {
                    game.LastCardPlayed = new LastCardPlayed(autoParams.DeflectParams.TargetedCardColor, deflectCard.Value, deflectCard.ImageUrl, autoParams.DeflectParams.PlayerTargeted.User.Name, true, deflectCard);
                    autoParams.DeflectParams.PlayerTargeted.Cards.Remove(deflectCard);
                    game.DiscardedPile.Add(deflectCard);
                    messageToLog += $"{autoParams.DeflectParams.PlayerTargeted.User.Name} deflected {autoParams.DeflectParams.CardPlayed.Value.ToString()}. {autoParams.DeflectParams.PlayerPlayed.User.Name} must draw {autoParams.DeflectParams.NumberOfCardsToDraw} cards.";

                    var kingsDecreeResult = BlockedByKingsDecree(messageToLog, autoParams.DeflectParams.PlayerPlayed,game);
                    messageToLog = kingsDecreeResult.MessageToLog;
                    if (!kingsDecreeResult.ActivatedKingsDecree)
                    {
                        _gameManager.DrawCard(game, autoParams.DeflectParams.PlayerPlayed, autoParams.DeflectParams.NumberOfCardsToDraw, false);
                    }
                }
            }

            return new AutomaticallyTriggeredResult() { MessageToLog = messageToLog };
        }


        private AutomaticallyTriggeredResult BlockedByKingsDecree(string messageToLog, Player player, Game game)
        {
            var activatedKingsDecree = false;

            if (game.SilenceTurnsRemaining <= 0)
            {
                if (player.Cards.Count > 4 && player.Cards.FirstOrDefault(x => x.Value == CardValue.KingsDecree) != null)
                {
                    activatedKingsDecree = true;
                    messageToLog += $"{player.User.Name} is not affected by the draw. He has more than 4 cards and king's decree in hand (auto effect is activated).";
                }
            }

            return new AutomaticallyTriggeredResult() { MessageToLog = messageToLog, ActivatedKingsDecree = activatedKingsDecree };
        }
    }
}