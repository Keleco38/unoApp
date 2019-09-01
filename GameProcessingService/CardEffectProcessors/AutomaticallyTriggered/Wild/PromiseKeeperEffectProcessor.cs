using Common.Enums;
using EntityObjects;
using GameProcessingService.CoreManagers;
using GameProcessingService.Models;

namespace GameProcessingService.CardEffectProcessors.AutomaticallyTriggered.Wild
{
    public class PromiseKeeperEffectProcessor:IAutomaticallyTriggeredCardEffectProcessor
    {
        private readonly IGameManager _gameManager;
        public CardValue CardAffected => CardValue.PromiseKeeper;

        public PromiseKeeperEffectProcessor(IGameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public AutomaticallyTriggeredResult ProcessCardEffect(Game game, AutomaticallyTriggeredParams automaticallyTriggeredParams)
        {

            if (automaticallyTriggeredParams.MoveParams.PlayerPlayed.CardPromisedToDiscard != null)
            {
                if (automaticallyTriggeredParams.MoveParams.CardPlayed.Color == automaticallyTriggeredParams.MoveParams.PlayerPlayed.CardPromisedToDiscard.Color && automaticallyTriggeredParams.MoveParams.CardPlayed.Value == automaticallyTriggeredParams.MoveParams.PlayerPlayed.CardPromisedToDiscard.Value)
                {
                    automaticallyTriggeredParams.MessageToLog = $"{automaticallyTriggeredParams.MoveParams.PlayerPlayed.User.Name} fulfilled their promise, they will discard one card. ";
                    if (automaticallyTriggeredParams.MoveParams.PlayerPlayed.Cards.Count > 0)
                    {
                        automaticallyTriggeredParams.MoveParams.PlayerPlayed.Cards.RemoveRange(0, 1);
                    }
                }
                else
                {
                    automaticallyTriggeredParams.MessageToLog = $"{automaticallyTriggeredParams.MoveParams.PlayerPlayed.User.Name} didn't fulfill their promise, they will draw 2 cards. ";
                    _gameManager.DrawCard(game, automaticallyTriggeredParams.MoveParams.PlayerPlayed, 2, false);
                }
                automaticallyTriggeredParams.MoveParams.PlayerPlayed.CardPromisedToDiscard = null;
            }

            return new AutomaticallyTriggeredResult(automaticallyTriggeredParams.MessageToLog);
        }
    }
}