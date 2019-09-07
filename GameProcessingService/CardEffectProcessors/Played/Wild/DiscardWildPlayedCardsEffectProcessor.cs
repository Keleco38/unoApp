using System.Collections.Generic;
using System.Linq;
using Common.Enums;
using EntityObjects;
using GameProcessingService.CoreManagers;
using GameProcessingService.Models;

namespace GameProcessingService.CardEffectProcessors.Played.Wild
{
    public class DiscardWildPlayedCardsEffectProcessor : IPlayedCardEffectProcessor
    {

        private readonly IGameManager _gameManager;
        public CardValue CardAffected => CardValue.DiscardWildCards;

        public DiscardWildPlayedCardsEffectProcessor(IGameManager gameManager)
        {
            _gameManager = gameManager;
        }


        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams, string messageToLog)
        {

            game.Players.ForEach(x =>
            {
                var wildCards = x.Cards.Where(y => y.Color == CardColor.Wild).ToList();
                game.DiscardedPile.AddRange(wildCards);
                wildCards.ForEach(y => x.Cards.Remove(y));
            });
            messageToLog +=($"{moveParams.PlayerPlayed.User.Name}  played discard all wildcards.");
            _gameManager.DrawCard(game, moveParams.PlayerPlayed, 1, false);

            return new MoveResult(messageToLog);
        }
    }
}