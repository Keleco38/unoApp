using System.Collections.Generic;
using System.Linq;
using Common.Enums;
using EntityObjects;
using GameProcessingService.CoreManagers;
using GameProcessingService.Models;

namespace GameProcessingService.CardEffectProcessors.Played.Wild
{
    public class HandOfGodEffectProcessor : IPlayedCardEffectProcessor
    {

        private readonly IGameManager _gameManager;
        public CardValue CardAffected => CardValue.HandOfGod;

        public HandOfGodEffectProcessor(IGameManager gameManager)
        {
            _gameManager = gameManager;
        }


        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {
            var messagesToLog = new List<string>();
            if (moveParams.PlayerPlayed.Cards.Count > 7)
            {
                messagesToLog.Add($"{moveParams.PlayerPlayed.User.Name} discarded 4 cards (hand of god). ");
                var cards = moveParams.PlayerPlayed.Cards.Take(4).ToList();
                game.DiscardedPile.AddRange(cards);
                cards.ForEach(y => moveParams.PlayerPlayed.Cards.Remove(y));
            }
            else
            {
                messagesToLog.Add($"{moveParams.PlayerPlayed.User.Name} didn't discard any cards. They had less than 8 cards. (hand of god)");
            }
            return new MoveResult(messagesToLog);
        }
    }
}