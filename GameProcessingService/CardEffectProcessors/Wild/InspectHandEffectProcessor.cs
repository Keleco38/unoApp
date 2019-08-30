using System.Collections.Generic;
using Common.Enums;
using System.Linq;
using Common.Contants;
using EntityObjects;
using GameProcessingService.CoreManagers;
using GameProcessingService.Models;

namespace GameProcessingService.CardEffectProcessors.Wild
{
    public class InspectHandEffectProcessor : ICardEffectProcessor
    {

        private readonly IGameManager _gameManager;
        public CardValue CardAffected => CardValue.InspectHand;

        public InspectHandEffectProcessor(IGameManager gameManager)
        {
            _gameManager = gameManager;
        }


        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {
            var messagesToLog = new List<string>();
            var moveResultCallbackParams = new List<MoveResultCallbackParam>();

            var messageToLog = ($"{moveParams.PlayerPlayed.User.Name} targeted {moveParams.PlayerTargeted.User.Name} with inspect hand. ");

            Player loopingPlayer = _gameManager.GetNextPlayer(game, moveParams.PlayerPlayed, game.Players);
            var playerExcludingPlayerPlaying = game.Players.Where(p => p != moveParams.PlayerPlayed).ToList();
            for (int i = 0; i < playerExcludingPlayerPlaying.Count; i++)
            {
                if (i != 0)
                {
                    loopingPlayer = _gameManager.GetNextPlayer(game, loopingPlayer, playerExcludingPlayerPlaying);
                }

                var magneticCard = loopingPlayer.Cards.FirstOrDefault(c => c.Value == CardValue.MagneticPolarity);
                if (magneticCard != null)
                {
                    game.LastCardPlayed = new LastCardPlayed(moveParams.TargetedCardColor, magneticCard.Value, magneticCard.ImageUrl, loopingPlayer.User.Name, true);
                    loopingPlayer.Cards.Remove(magneticCard);
                    game.DiscardedPile.Add(magneticCard);
                    messageToLog += ($"{loopingPlayer.User.Name} intercepted attack with magnetic polarity. ");
                    moveParams.PlayerTargeted = loopingPlayer;
                    break;
                }
            }

            if (game.PlayerToPlay.Cards.Any())
            {
                var moveResultCallbackParam = new MoveResultCallbackParam(Constants.SHOW_CARDS_CALLBACK_COMMAND, moveParams.PlayerPlayed.User.ConnectionId, moveParams.PlayerTargeted.Cards);
                moveResultCallbackParams.Add(moveResultCallbackParam);
            }

            messageToLog += ($"{moveParams.PlayerPlayed.User.Name} inspected {moveParams.PlayerTargeted.User.Name}'s hand.");
            messagesToLog.Add(messageToLog);

            return new MoveResult(messagesToLog, moveResultCallbackParams);
        }
    }
}