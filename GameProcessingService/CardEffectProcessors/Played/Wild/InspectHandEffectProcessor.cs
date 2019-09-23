using System.Collections.Generic;
using System.Linq;
using Common.Contants;
using Common.Enums;
using EntityObjects;
using EntityObjects.Cards.Abstraction;
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


        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {
            var messagesToLog = new List<string>();
            var moveResultCallbackParams = new List<MoveResultCallbackParam>();

            var messageToLog = ($"{moveParams.PlayerPlayed.User.Name} targeted {moveParams.PlayerTargeted.User.Name} with inspect hand. ");

            var automaticallyTriggeredResultMagneticPolarity = _automaticallyTriggeredCardEffectProcessors.First(x => x.CardAffected == CardValue.MagneticPolarity).ProcessCardEffect(game, messageToLog, new AutomaticallyTriggeredParams() { MagneticPolarityParams = new AutomaticallyTriggeredMagneticPolarityParams(moveParams.TargetedCardColor, moveParams.PlayerPlayed, moveParams.PlayerTargeted) });
            moveParams.PlayerTargeted = automaticallyTriggeredResultMagneticPolarity.MagneticPolaritySelectedPlayer;
            messageToLog = automaticallyTriggeredResultMagneticPolarity.MessageToLog;

            if (game.PlayerToPlay.Cards.Any())
            {

                List<KeyValuePair<string, List<ICard>>> result = new List<KeyValuePair<string, List<ICard>>>
                {
                    new KeyValuePair<string, List<ICard>>($"{moveParams.PlayerTargeted.User.Name}'s cards", moveParams.PlayerTargeted.Cards.OrderBy(y => y.Color).ThenBy(y => y.Value).ToList())
                };

                var moveResultCallbackParam = new MoveResultCallbackParam(Constants.Commands.SHOW_CARDS_CALLBACK_COMMAND, moveParams.PlayerPlayed.User.ConnectionId, result);
                moveResultCallbackParams.Add(moveResultCallbackParam);
            }

            messageToLog += ($"{moveParams.PlayerPlayed.User.Name} inspected {moveParams.PlayerTargeted.User.Name}'s hand.");
            messagesToLog.Add(messageToLog);

            return new MoveResult(messagesToLog, moveResultCallbackParams);
        }
    }
}