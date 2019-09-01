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

        public AutomaticallyTriggeredResult ProcessCardEffect(Game game, AutomaticallyTriggeredParams automaticallyTriggeredParams)
        {
            automaticallyTriggeredParams.PlayersAffected.ForEach(x =>
            {
                var doubleDrawCard = x.Cards.FirstOrDefault(c => c.Value == CardValue.DoubleDraw);
                if (doubleDrawCard != null)
                {
                    automaticallyTriggeredParams.NumberOfCardsToDraw = automaticallyTriggeredParams.NumberOfCardsToDraw * 2;
                    automaticallyTriggeredParams.Game.LastCardPlayed = new LastCardPlayed(automaticallyTriggeredParams.MoveParams.TargetedCardColor, doubleDrawCard.Value, doubleDrawCard.ImageUrl, x.User.Name, true);
                    x.Cards.Remove(doubleDrawCard);
                    automaticallyTriggeredParams.Game.DiscardedPile.Add(doubleDrawCard);
                    automaticallyTriggeredParams.MessageToLog += $"{x.User.Name} doubled the draw effect. ";
                }
            });
            return new AutomaticallyTriggeredResult(automaticallyTriggeredParams.MessageToLog);
        }
    }
}