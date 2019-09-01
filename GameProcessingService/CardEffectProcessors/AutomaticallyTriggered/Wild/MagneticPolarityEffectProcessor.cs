using System.Linq;
using Common.Enums;
using EntityObjects;
using GameProcessingService.CoreManagers;
using GameProcessingService.Models;

namespace GameProcessingService.CardEffectProcessors.AutomaticallyTriggered.Wild
{
    public class MagneticPolarityEffectProcessor : IAutomaticallyTriggeredCardEffectProcessor
    {
        private readonly IGameManager _gameManager;
        public CardValue CardAffected => CardValue.MagneticPolarity;

        public MagneticPolarityEffectProcessor(IGameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public AutomaticallyTriggeredResult ProcessCardEffect(Game game, AutomaticallyTriggeredParams automaticallyTriggeredParams)
        {
            Player loopingPlayer = _gameManager.GetNextPlayer(game, automaticallyTriggeredParams.MoveParams.PlayerPlayed, game.Players);
            var playerExcludingPlayerPlaying = game.Players.Where(p => p != automaticallyTriggeredParams.MoveParams.PlayerPlayed).ToList();
            for (int i = 0; i < playerExcludingPlayerPlaying.Count; i++)
            {
                if (i != 0)
                {
                    loopingPlayer = _gameManager.GetNextPlayer(game, loopingPlayer, playerExcludingPlayerPlaying);
                }

                var magneticCard = loopingPlayer.Cards.FirstOrDefault(c => c.Value == CardValue.MagneticPolarity);
                if (magneticCard != null)
                {
                    game.LastCardPlayed = new LastCardPlayed(automaticallyTriggeredParams.MoveParams.TargetedCardColor, magneticCard.Value, magneticCard.ImageUrl, loopingPlayer.User.Name, true);
                    loopingPlayer.Cards.Remove(magneticCard);
                    game.DiscardedPile.Add(magneticCard);
                    automaticallyTriggeredParams.MessageToLog += ($"{loopingPlayer.User.Name} intercepted the attack (magnetic polarity). ");
                    automaticallyTriggeredParams.MoveParams.PlayerTargeted = loopingPlayer;
                    break;
                }
            }

            return new AutomaticallyTriggeredResult(automaticallyTriggeredParams.MessageToLog);
        }
    }
}