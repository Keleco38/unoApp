using System.Collections.Generic;
using System.Linq;
using EntityObjects;
using GameProcessingService.GameManager;
using GameProcessingService.Models;

namespace GameProcessingService.CardEffectProcessors.Wild
{
    public class HandOfGodEffectProcessor:ICardEffectProcessor
    {

        private readonly IGameManager _gameManager;

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