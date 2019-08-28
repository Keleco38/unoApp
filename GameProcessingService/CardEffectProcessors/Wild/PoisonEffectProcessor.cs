using System.Collections.Generic;
using System.Linq;
using Common.Enums;
using EntityObjects;
using GameProcessingService.CoreManagers.GameManagers;
using GameProcessingService.Models;

namespace GameProcessingService.CardEffectProcessors.Wild
{
    public class PoisonEffectProcessor : ICardEffectProcessor
    {

        private readonly IGameManager _gameManager;

        public PoisonEffectProcessor(IGameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {

            var messagesToLog = new List<string>();
            var messageToLog = $"{moveParams.PlayerPlayed.User.Name} changed color to {moveParams.TargetedCardColor} (poison card). ";

            var doubleDrawCard = moveParams.PlayerPlayed.Cards.FirstOrDefault(c => c.Value == CardValue.DoubleDraw);
            if (doubleDrawCard != null)
            {
                _gameManager.DrawCard(game,moveParams.PlayerPlayed, 4, false);
                game.LastCardPlayed = new LastCardPlayed(moveParams.TargetedCardColor, doubleDrawCard.Value, doubleDrawCard.ImageUrl, moveParams.PlayerPlayed.User.Name, true);
                moveParams.PlayerPlayed.Cards.Remove(doubleDrawCard);
                game.DiscardedPile.Add(doubleDrawCard);
                messageToLog += $"{moveParams.PlayerPlayed.User.Name} had double draw, he drew 4 cards. ";
            }
            else
            {
                _gameManager.DrawCard(game,moveParams.PlayerPlayed, 2, false);
                messageToLog += $" {moveParams.PlayerPlayed.User.Name} drew 2 cards. ";
            }
            messagesToLog.Add(messageToLog);
            return new MoveResult(messagesToLog);
        }
    }
}
