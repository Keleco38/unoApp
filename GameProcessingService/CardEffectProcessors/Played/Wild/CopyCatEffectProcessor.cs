using System.Collections.Generic;
using System.Linq;
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
            var cardToCopy = moveParams.PreviousLastCardPlayed.OriginalCardPlayer;
            var messageToLog = $"{moveParams.PlayerPlayed.User.Name} played copycat. They copied {cardToCopy.Value.ToString()} and kept their turn.";
            if (game.CardValuesRemovedFromTheRound.Contains(cardToCopy.Value))
            {
                messageToLog += "Card was not copied, it was removed from the game by the death sentence.";
            }
            else
            {
                moveParams.PlayerPlayed.Cards.Add(cardToCopy);
                if (game.DiscardedPile.Contains(cardToCopy))
                {
                    game.DiscardedPile.Remove(cardToCopy);
                }
            }
            var previousPlayer = _gameManager.GetNextPlayer(game, moveParams.PlayerPlayed, game.Players, true);
            game.PlayerToPlay = previousPlayer;
            messagesToLog.Add(messageToLog);
            return new MoveResult(messagesToLog);
        }
    }
}