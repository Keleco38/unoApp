using System.Collections.Generic;
using System.Linq;
using Common.Enums;
using EntityObjects;
using GameProcessingService.CoreManagers;
using GameProcessingService.Models;

namespace GameProcessingService.CardEffectProcessors.Played.Wild
{
    public class HopeEffectProcessor : IPlayedCardEffectProcessor
    {
        private readonly IGameManager _gameManager;
        public CardValue CardAffected => CardValue.Hope;

        public HopeEffectProcessor(IGameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {
            var messagesToLog = new List<string>();
            var cardToCopy = moveParams.PreviousLastCardPlayed.OriginalCardPlayer;
            var messageToLog = $"{moveParams.PlayerPlayed.User.Name} played Hope. ";

            var cardDrew = game.Deck.Cards.FirstOrDefault();

            if (cardDrew == null)
            {
                cardDrew = game.DiscardedPile.First();
                game.DiscardedPile.Remove(cardDrew);
            }
            else
            {
                game.Deck.Cards.Remove(cardDrew);
            }

            if (cardDrew.Color == CardColor.Wild)
            {
                messageToLog += $"They drew {cardDrew.Value.ToString()} and kept the card. {moveParams.PlayerPlayed.User.Name} will also keep their turn to play. ";
                moveParams.PlayerPlayed.Cards.Add(cardDrew);
                var previousPlayer = _gameManager.GetNextPlayer(game, moveParams.PlayerPlayed, game.Players, true);
                game.PlayerToPlay = previousPlayer; 
            }
            else
            {
                messageToLog += "They didn't draw a wildcard, the card is discarded";
                game.DiscardedPile.Add(cardDrew);
            }

            messagesToLog.Add(messageToLog);
            return new MoveResult(messagesToLog);
        }
    }
}