using System.Collections.Generic;
using Common.Enums;
using EntityObjects;
using GameProcessingService.CoreManagers;
using GameProcessingService.Models;

namespace GameProcessingService.CardEffectProcessors.Played.Wild
{
    public class PromiseKeeperEffectProcessor : IPlayedCardEffectProcessor
    {

        private readonly IGameManager _gameManager;
        public CardValue CardAffected => CardValue.PromiseKeeper;

        public PromiseKeeperEffectProcessor(IGameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {
            var messagesToLog = new List<string>();
            moveParams.PlayerPlayed.CardPromisedToDiscard = moveParams.CardPromisedToDiscard;
            messagesToLog.Add($"{moveParams.PlayerPlayed.User.Name} changed color to {moveParams.TargetedCardColor} (promise keeper). They also picked a card he will discard next turn.");
            return new MoveResult(messagesToLog);
        }
    }
}