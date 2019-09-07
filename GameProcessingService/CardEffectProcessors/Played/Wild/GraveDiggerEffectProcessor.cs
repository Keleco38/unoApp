using System.Collections.Generic;
using Common.Enums;
using EntityObjects;
using GameProcessingService.CoreManagers;
using GameProcessingService.Models;

namespace GameProcessingService.CardEffectProcessors.Played.Wild
{
    public class GraveDiggerEffectProcessor : IPlayedCardEffectProcessor
    {
        private readonly IGameManager _gameManager;
        public CardValue CardAffected => CardValue.GraveDigger;

        public GraveDiggerEffectProcessor(IGameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams, string messageToLog)
        {
            moveParams.PlayerPlayed.Cards.Add(moveParams.CardToDig);
            game.DiscardedPile.Remove(moveParams.CardToDig);
            messageToLog += ($"{moveParams.PlayerPlayed.User.Name} dug card {moveParams.CardToDig.Color.ToString()} {moveParams.CardToDig.Value.ToString()} from the discarded pile.");
            return new MoveResult(messageToLog);
        }
    }
}