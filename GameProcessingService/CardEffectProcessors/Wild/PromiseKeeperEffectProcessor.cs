using System.Collections.Generic;
using EntityObjects;
using GameProcessingService.CoreManagers.GameManagers;
using GameProcessingService.Models;

namespace GameProcessingService.CardEffectProcessors.Wild
{
    public class PromiseKeeperEffectProcessor:ICardEffectProcessor
    {

        private readonly IGameManager _gameManager;

        public PromiseKeeperEffectProcessor(IGameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {
            var messagesToLog = new List<string>();
            moveParams.PlayerPlayed.CardPromisedToDiscard = moveParams.CardPromisedToDiscard;
            messagesToLog.Add($"{moveParams.PlayerPlayed.User.Name} changed color to {moveParams.TargetedCardColor} (promise keeper). He also picked a card he will discard next turn.");
            return new MoveResult(messagesToLog);
        }
    }
}