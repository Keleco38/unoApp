using System.Collections.Generic;
using System.Linq;
using Common.Enums;
using EntityObjects;
using GameProcessingService.GameManager;
using GameProcessingService.Models;

namespace GameProcessingService.CardEffectProcessors.Wild
{
    public class DiscardWildCardsEffectProcessor:ICardEffectProcessor
    {

        private readonly IGameManager _gameManager;

        public DiscardWildCardsEffectProcessor(IGameManager gameManager)
        {
            _gameManager = gameManager;
        }


        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {
            var messagesToLog = new List<string>();

            game.Players.ForEach(x =>
            {
                var wildCards = x.Cards.Where(y => y.Color == CardColor.Wild).ToList();
                game.DiscardedPile.AddRange(wildCards);
                wildCards.ForEach(y => x.Cards.Remove(y));
            });
            messagesToLog.Add($"{moveParams.PlayerPlayed.User.Name}  played discard all wildcards.");
            _gameManager.DrawCard(game,moveParams.PlayerPlayed, 1, false);

            return new MoveResult(messagesToLog);
        }
    }
}