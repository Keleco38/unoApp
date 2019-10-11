using System.Collections.Generic;
using System.Linq;
using Common.Contants;
using Common.Enums;
using EntityObjects;
using EntityObjects.Cards.Abstraction;
using GameProcessingService.CoreManagers;
using GameProcessingService.Models;

namespace GameProcessingService.CardEffectProcessors.Played.Wild
{
    public class FortuneTellerEffectProcessor : IPlayedCardEffectProcessor
    {
        private readonly IGameManager _gameManager;
        public CardValue CardAffected => CardValue.FortuneTeller;

        public FortuneTellerEffectProcessor(IGameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {
            var callbackParams = new List<MoveResultCallbackParam>();
            var messagesToLog = new List<string>();
            var messageToLog = $"{moveParams.PlayerPlayed.User.Name} inspected top 5 cards from the deck";
            messagesToLog.Add(messageToLog);

            if (game.PlayerToPlay.Cards.Any() || game.Players.Any(x=>x.Cards.Any(c=>c.Value==CardValue.TheLastStand)))
            {
                List<KeyValuePair<string, List<ICard>>> result = new List<KeyValuePair<string, List<ICard>>>();
                result.Add(new KeyValuePair<string, List<ICard>>("Deck's' top 5 cards", game.Deck.Cards.Take(5).ToList()));

                callbackParams.Add(new MoveResultCallbackParam(Constants.Commands.SHOW_CARDS_CALLBACK_COMMAND, moveParams.PlayerPlayed.User.ConnectionId, result));
            }

            return new MoveResult(messagesToLog, callbackParams);
        }
    }
}