using System.Collections.Generic;
using System.Linq;
using Common.Contants;
using Common.Enums;
using EntityObjects;
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

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams, string messageToLog)
        {
            var callbackParams = new List<MoveResultCallbackParam>();
            messageToLog += $"{moveParams.PlayerPlayed.User.Name} inspected top 5 cards from the deck";

            if (game.PlayerToPlay.Cards.Any())
            {
                callbackParams.Add(new MoveResultCallbackParam(Constants.SHOW_CARDS_CALLBACK_COMMAND, moveParams.PlayerPlayed.User.ConnectionId, game.Deck.Cards.Take(5)));
            }

            return new MoveResult(messageToLog, callbackParams);
        }
    }
}