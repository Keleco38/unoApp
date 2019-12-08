using System.Linq;
using Common.Enums;
using EntityObjects;
using GameProcessingService.CoreManagers;
using GameProcessingService.Models;

namespace GameProcessingService.CardEffectProcessors.AutomaticallyTriggered.Wild
{
    public class KingsDecreeEffectProcessor: IAutomaticallyTriggeredCardEffectProcessor
    {
        private readonly IGameManager _gameManager;
        public CardValue CardAffected => CardValue.KingsDecree;

        public KingsDecreeEffectProcessor(IGameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public AutomaticallyTriggeredResult ProcessCardEffect(Game game,  string messageToLog, AutomaticallyTriggeredParams autoParams)
        {
            var player = autoParams.KingsDecreeParams.PlayerAffected;
            var activatedKingsDecree = false;

            if (player.Cards.Count > 4 && player.Cards.FirstOrDefault(x => x.Value == CardValue.KingsDecree)!=null)
            {
                activatedKingsDecree = true;
                messageToLog += $"{player.User.Name} is not affected by the draw. He has more than 4 cards and king's decree in hand.";
            }

            return new AutomaticallyTriggeredResult(){MessageToLog = messageToLog, ActivatedKingsDecree = activatedKingsDecree };
        }
    }
}