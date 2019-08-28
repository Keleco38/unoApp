using System.Collections.Generic;
using EntityObjects;
using GameProcessingService.CoreManagers.GameManagers;
using GameProcessingService.Models;

namespace GameProcessingService.CardEffectProcessors.Wild
{
    public class GraveDiggerEffectProcessor:ICardEffectProcessor
    {
        private readonly IGameManager _gameManager;

        public GraveDiggerEffectProcessor(IGameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {
            var messagesToLog = new List<string>();
            moveParams.PlayerPlayed.Cards.Add(moveParams.CardToDig);
            game.DiscardedPile.Remove(moveParams.CardToDig);
            messagesToLog.Add($"{moveParams.PlayerPlayed.User.Name} dug card {moveParams.CardToDig.Color.ToString()} {moveParams.CardToDig.Value.ToString()} from the discarded pile.");
            return new MoveResult(messagesToLog);
        }
    }
}