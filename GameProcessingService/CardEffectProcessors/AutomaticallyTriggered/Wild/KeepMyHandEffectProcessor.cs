using System.Collections.Generic;
using System.Linq;
using Common.Enums;
using EntityObjects;
using GameProcessingService.CoreManagers;
using GameProcessingService.Models;

namespace GameProcessingService.CardEffectProcessors.AutomaticallyTriggered.Wild
{
    public class KeepMyHandEffectProcessor : IAutomaticallyTriggeredCardEffectProcessor
    {
        private readonly IGameManager _gameManager;
        public CardValue CardAffected => CardValue.KeepMyHand;

        public KeepMyHandEffectProcessor(IGameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public AutomaticallyTriggeredResult ProcessCardEffect(Game game, string messageToLog, AutomaticallyTriggeredParams autoParams)
        {
            var playersWithoutKeepMyHand = new List<Player>();

            autoParams.KeepMyHandParams.PlayersAffected.ForEach(p =>
            {
                var keepMyHandCard = p.Cards.FirstOrDefault(y => y.Value == CardValue.KeepMyHand);
                if (keepMyHandCard != null)
                {
                    game.LastCardPlayed = new LastCardPlayed(autoParams.KeepMyHandParams.TargetedCardColor, keepMyHandCard.Value, keepMyHandCard.ImageUrl, p.User.Name, true, keepMyHandCard);
                    p.Cards.Remove(keepMyHandCard);
                    game.DiscardedPile.Add(keepMyHandCard);
                    messageToLog += $"{p.User.Name} kept their hand safe. ";
                }
                else
                {
                    playersWithoutKeepMyHand.Add(p);
                }
            });



            return new AutomaticallyTriggeredResult() { MessageToLog = messageToLog, PlayersWithoutKeepMyHand = playersWithoutKeepMyHand };
        }
    }
}