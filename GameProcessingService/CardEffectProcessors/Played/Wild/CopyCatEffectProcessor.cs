using System.Collections.Generic;
using Common.Enums;
using EntityObjects;
using GameProcessingService.CoreManagers;
using GameProcessingService.Models;

namespace GameProcessingService.CardEffectProcessors.Played.Wild
{
    public class CopyCatEffectProcessor : IPlayedCardEffectProcessor
    {
        private readonly IGameManager _gameManager;
        public CardValue CardAffected => CardValue.CopyCat;

        public CopyCatEffectProcessor(IGameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {
            var messagesToLog = new List<string>();
            var cardToCopy = game.DiscardedPile[^2];
            var messageToLog = $"{moveParams.PlayerPlayed.User.Name} played copycat. He copied {cardToCopy.Value.ToString()} and kept his turn.";
            moveParams.PlayerPlayed.Cards.Add(cardToCopy);
            var previousPlayer = _gameManager.GetNextPlayer(game, moveParams.PlayerPlayed, game.Players, true);
            game.PlayerToPlay = previousPlayer;
            messagesToLog.Add(messageToLog);
            return new MoveResult(messagesToLog);
        }
    }
}