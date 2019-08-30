using System;
using System.Collections.Generic;
using Common.Enums;
using System.Linq;
using EntityObjects;
using GameProcessingService.CoreManagers;
using GameProcessingService.Models;

namespace GameProcessingService.CardEffectProcessors.Wild
{
    public class DiscardColorEffectProcessor : ICardEffectProcessor
    {
        private readonly IGameManager _gameManager;
        public CardValue CardAffected => CardValue.DiscardColor;

        public DiscardColorEffectProcessor(IGameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {
            var messagesToLog = new List<string>();
            game.Players.ForEach(p =>
            {
                var cardsInThatColor = p.Cards.Where(y => y.Color == moveParams.TargetedCardColor).ToList();
                game.DiscardedPile.AddRange(cardsInThatColor);
                cardsInThatColor.ForEach(y => p.Cards.Remove(y));

            });
            Random random = new Random();
            var colorIds = new int[] { 1, 2, 3, 4 };
            int randomColor = colorIds[(random.Next(4))];
            game.LastCardPlayed.Color = (CardColor)randomColor;
            _gameManager.DrawCard(game, moveParams.PlayerPlayed, 1, false);
            messagesToLog.Add($"{moveParams.PlayerPlayed.User.Name} played discard color. All players discarded {moveParams.TargetedCardColor} and a random color has been assigned.");
            return new MoveResult(messagesToLog);
        }
    }
}