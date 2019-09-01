using System.Linq;
using Common.Enums;
using EntityObjects;
using GameProcessingService.CoreManagers;
using GameProcessingService.Models;

namespace GameProcessingService.CardEffectProcessors.AutomaticallyTriggered.Wild
{
    public class KeepMyHandEffectProcessor: IAutomaticallyTriggeredCardEffectProcessor
    {
        private readonly IGameManager _gameManager;
        public CardValue CardAffected => CardValue.KeepMyHand;

        public KeepMyHandEffectProcessor(IGameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public AutomaticallyTriggeredResult ProcessCardEffect(Game game, AutomaticallyTriggeredParams automaticallyTriggeredParams)
        {
            automaticallyTriggeredParams.PlayersAffected.ForEach(p =>
            {
                var keepMyHandCard = p.Cards.FirstOrDefault(y => y.Value == CardValue.KeepMyHand);
                if (keepMyHandCard != null)
                {
                    game.LastCardPlayed = new LastCardPlayed(automaticallyTriggeredParams.MoveParams.TargetedCardColor, keepMyHandCard.Value, keepMyHandCard.ImageUrl, p.User.Name, true);
                    p.Cards.Remove(keepMyHandCard);
                    game.DiscardedPile.Add(keepMyHandCard);
                    automaticallyTriggeredParams.MessageToLog += $"{p.User.Name} kept their hand safe. ";
                    automaticallyTriggeredParams.PlayersAffected.Remove(p);
                }
            });

            return new AutomaticallyTriggeredResult(automaticallyTriggeredParams.MessageToLog,0,automaticallyTriggeredParams.PlayersAffected);
        }
    }
}