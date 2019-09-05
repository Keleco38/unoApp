using System.Linq;
using Common.Enums;
using EntityObjects;
using GameProcessingService.CoreManagers;
using GameProcessingService.Models;

namespace GameProcessingService.CardEffectProcessors.AutomaticallyTriggered.Wild
{
    public class TheLastStandEffectProcessor : IAutomaticallyTriggeredCardEffectProcessor
    {
        private readonly IGameManager _gameManager;
        public CardValue CardAffected => CardValue.TheLastStand;

        public TheLastStandEffectProcessor(IGameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public AutomaticallyTriggeredResult ProcessCardEffect(Game game, string messageToLog, AutomaticallyTriggeredParams autoParams)
        {
            var playersWithoutCards = game.Players.Where(x => !x.Cards.Any()).ToList();

            var firstPlayerWithTheLastStand = game.Players.Where(x => x.Cards.Any()).FirstOrDefault(x => x.Cards.FirstOrDefault(y => y.Value == CardValue.TheLastStand) != null);

            if (firstPlayerWithTheLastStand != null)
            {
                var theLastStandCard = firstPlayerWithTheLastStand.Cards.First(x => x.Value == CardValue.TheLastStand);
                game.LastCardPlayed = new LastCardPlayed(game.LastCardPlayed.Color, theLastStandCard.Value, theLastStandCard.ImageUrl, game.PlayerToPlay.User.Name, true);
                firstPlayerWithTheLastStand.Cards.Remove(theLastStandCard);
                game.DiscardedPile.Add(theLastStandCard);
                messageToLog = $"{firstPlayerWithTheLastStand.User.Name} saved the day! They played The Last Stand. Every player that had 0 cards will draw 2 cards.";
                foreach (var player in playersWithoutCards)
                {
                    _gameManager.DrawCard(game, player, 2, false);
                }
            }

            return new AutomaticallyTriggeredResult() { MessageToLog = messageToLog };
        }
    }
}