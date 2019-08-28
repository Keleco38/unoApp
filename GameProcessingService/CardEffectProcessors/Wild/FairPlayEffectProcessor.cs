using System.Collections.Generic;
using System.Linq;
using Common.Enums;
using EntityObjects;
using GameProcessingService.GameManager;
using GameProcessingService.Models;

namespace GameProcessingService.CardEffectProcessors.Wild
{
    public class FairPlayEffectProcessor:ICardEffectProcessor
    {
        private readonly IGameManager _gameManager;

        public FairPlayEffectProcessor(IGameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {
            var messagesToLog = new List<string>();
            var messageToLog = $"{moveParams.PlayerPlayed.User.Name} targeted {moveParams.PlayerTargeted.User.Name} with Fair Play. ";

            Player loopingPlayer = _gameManager.GetNextPlayer(game,moveParams.PlayerPlayed, game.Players);
            var playerExcludingPlayerPlaying = game.Players.Where(p => p != moveParams.PlayerPlayed).ToList();
            for (int i = 0; i < playerExcludingPlayerPlaying.Count; i++)
            {
                if (i != 0)
                {
                    loopingPlayer = _gameManager.GetNextPlayer(game,loopingPlayer, playerExcludingPlayerPlaying);
                }

                var magneticCard = loopingPlayer.Cards.FirstOrDefault(c => c.Value == CardValue.MagneticPolarity);
                if (magneticCard != null)
                {
                    game.LastCardPlayed = new LastCardPlayed(moveParams.TargetedCardColor, magneticCard.Value, magneticCard.ImageUrl, loopingPlayer.User.Name, true);
                    loopingPlayer.Cards.Remove(magneticCard);
                    game.DiscardedPile.Add(magneticCard);
                    messageToLog += ($"{loopingPlayer.User.Name} intercepted attack with magnetic polarity. ");
                    moveParams.PlayerTargeted = loopingPlayer;
                    break;
                }
            }

            var cardDifference = moveParams.PlayerTargeted.Cards.Count - moveParams.PlayerPlayed.Cards.Count;

            if (cardDifference == 0)
            {
                messageToLog += "No effect. They had the same number of cards.";
            }
            else if (cardDifference > 0)
            {
                //targeted Player discards
                moveParams.PlayerTargeted.Cards.RemoveRange(0, cardDifference);
                messageToLog += $"{moveParams.PlayerTargeted.User.Name} discarded {cardDifference} cards. They had more cards.";
            }
            else
            {
                //targeted Player draws
                var numberInPositiveValue = cardDifference * -1;
                _gameManager.DrawCard(game,moveParams.PlayerTargeted, numberInPositiveValue, false);
                messageToLog += $"{moveParams.PlayerTargeted.User.Name} must draw {numberInPositiveValue} cards. They had less cards.";
            }
            messagesToLog.Add(messageToLog);
            return new MoveResult(messagesToLog);
        }
    }
}