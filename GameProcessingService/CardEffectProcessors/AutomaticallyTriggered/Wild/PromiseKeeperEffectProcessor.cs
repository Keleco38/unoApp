using System.Linq;
using Common.Enums;
using Common.Extensions;
using EntityObjects;
using GameProcessingService.CoreManagers;
using GameProcessingService.Models;

namespace GameProcessingService.CardEffectProcessors.AutomaticallyTriggered.Wild
{
    public class PromiseKeeperEffectProcessor : IAutomaticallyTriggeredCardEffectProcessor
    {
        private readonly IGameManager _gameManager;
        public CardValue CardAffected => CardValue.PromiseKeeper;

        public PromiseKeeperEffectProcessor(IGameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public AutomaticallyTriggeredResult ProcessCardEffect(Game game, string messageToLog, AutomaticallyTriggeredParams autoParams)
        {

            if (autoParams.PromiseKeeperParams.PlayerPlayed.CardPromisedToDiscard != null)
            {
                if (autoParams.PromiseKeeperParams.CardPlayed.Color == autoParams.PromiseKeeperParams.PlayerPlayed.CardPromisedToDiscard.Color && autoParams.PromiseKeeperParams.CardPlayed.Value == autoParams.PromiseKeeperParams.PlayerPlayed.CardPromisedToDiscard.Value)
                {
                    messageToLog = $"{autoParams.PromiseKeeperParams.PlayerPlayed.User.Name} fulfilled their promise, they will discard one card. ";
                    if (autoParams.PromiseKeeperParams.PlayerPlayed.Cards.Count > 0)
                    {
                        var cardsToDiscard = autoParams.PromiseKeeperParams.PlayerPlayed.Cards.GetAndRemove(0, 1);
                        game.DiscardedPile.Add(cardsToDiscard.First());
                    }
                }
                else
                {
                    messageToLog = $"{autoParams.PromiseKeeperParams.PlayerPlayed.User.Name} didn't fulfill their promise, they will draw 2 cards. ";
                    _gameManager.DrawCard(game, autoParams.PromiseKeeperParams.PlayerPlayed, 2, false);
                }
                autoParams.PromiseKeeperParams.PlayerPlayed.CardPromisedToDiscard = null;
            }

            return new AutomaticallyTriggeredResult() { MessageToLog = messageToLog };
        }
    }
}