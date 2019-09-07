using System.Collections.Generic;
using System.Linq;
using Common.Contants;
using Common.Enums;
using EntityObjects;
using GameProcessingService.CardEffectProcessors.AutomaticallyTriggered;
using GameProcessingService.CoreManagers;
using GameProcessingService.Models;

namespace GameProcessingService.CardEffectProcessors.Played.Wild
{
    public class InspectHandEffectProcessor : IPlayedCardEffectProcessor
    {

        private readonly IGameManager _gameManager;
        private readonly IEnumerable<IAutomaticallyTriggeredCardEffectProcessor> _automaticallyTriggeredCardEffectProcessors;
        public CardValue CardAffected => CardValue.InspectHand;

        public InspectHandEffectProcessor(IGameManager gameManager, IEnumerable<IAutomaticallyTriggeredCardEffectProcessor> automaticallyTriggeredCardEffectProcessors)
        {
            _gameManager = gameManager;
            _automaticallyTriggeredCardEffectProcessors = automaticallyTriggeredCardEffectProcessors;
        }


        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams, string messageToLog)
        {
            var moveResultCallbackParams = new List<MoveResultCallbackParam>();

            messageToLog += ($"{moveParams.PlayerPlayed.User.Name} targeted {moveParams.PlayerTargeted.User.Name} with inspect hand. ");

            var automaticallyTriggeredResultMagneticPolarity = _automaticallyTriggeredCardEffectProcessors.First(x => x.CardAffected == CardValue.MagneticPolarity).ProcessCardEffect(game, messageToLog, new AutomaticallyTriggeredParams() { MagneticPolarityParams = new AutomaticallyTriggeredMagneticPolarityParams(moveParams.TargetedCardColor,moveParams.PlayerPlayed,moveParams.PlayerTargeted) });
            moveParams.PlayerTargeted = automaticallyTriggeredResultMagneticPolarity.MagneticPolaritySelectedPlayer;
            messageToLog = automaticallyTriggeredResultMagneticPolarity.MessageToLog;

            if (game.PlayerToPlay.Cards.Any())
            {
                var moveResultCallbackParam = new MoveResultCallbackParam(Constants.SHOW_CARDS_CALLBACK_COMMAND, moveParams.PlayerPlayed.User.ConnectionId, moveParams.PlayerTargeted.Cards);
                moveResultCallbackParams.Add(moveResultCallbackParam);
            }

            messageToLog += ($"{moveParams.PlayerPlayed.User.Name} inspected {moveParams.PlayerTargeted.User.Name}'s hand.");

            return new MoveResult(messageToLog, moveResultCallbackParams);
        }
    }
}