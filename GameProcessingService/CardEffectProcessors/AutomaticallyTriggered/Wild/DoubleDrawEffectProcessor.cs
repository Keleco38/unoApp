using System.Linq;
using Common.Enums;
using EntityObjects;
using GameProcessingService.CoreManagers;
using GameProcessingService.Models;

namespace GameProcessingService.CardEffectProcessors.AutomaticallyTriggered.Wild
{
    public class DoubleDrawEffectProcessor : IAutomaticallyTriggeredCardEffectProcessor
    {
        private readonly IGameManager _gameManager;
        public CardValue CardAffected => CardValue.DoubleDraw;

        public DoubleDrawEffectProcessor(IGameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public AutomaticallyTriggeredResult ProcessCardEffect(Game game, string messageToLog, AutomaticallyTriggeredParams autoParams)
        {
            var doubleDrawCard = autoParams.DoubleDrawParams.PlayerAffected.Cards.FirstOrDefault(c => c.Value == CardValue.DoubleDraw);
            if (doubleDrawCard != null)
            {
                autoParams.DoubleDrawParams.NumberOfCardsToDraw = autoParams.DoubleDrawParams.NumberOfCardsToDraw * 2;
                game.LastCardPlayed = new LastCardPlayed(autoParams.DoubleDrawParams.TargetedCardColor, doubleDrawCard.Value, doubleDrawCard.ImageUrl, autoParams.DoubleDrawParams.PlayerAffected.User.Name, true, doubleDrawCard);
                autoParams.DoubleDrawParams.PlayerAffected.Cards.Remove(doubleDrawCard);
                game.DiscardedPile.Add(doubleDrawCard);
                messageToLog += $"{autoParams.DoubleDrawParams.PlayerAffected.User.Name} doubled the draw/discard effect. ";
            }

            return new AutomaticallyTriggeredResult() { MessageToLog = messageToLog, NumberOfCardsToDraw = autoParams.DoubleDrawParams.NumberOfCardsToDraw };
        }
    }
}