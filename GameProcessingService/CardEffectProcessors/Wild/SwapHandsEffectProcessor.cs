using System.Collections.Generic;
using System.Linq;
using Common.Enums;
using EntityObjects;
using GameProcessingService.GameManager;
using GameProcessingService.Models;

namespace GameProcessingService.CardEffectProcessors.Wild
{
    public class SwapHandsEffectProcessor:ICardEffectProcessor
    {
        private readonly IGameManager _gameManager;

        public SwapHandsEffectProcessor(IGameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {
            var messagesToLog = new List<string>();
            var messageToLog = $"{moveParams.PlayerPlayed.User.Name} targeted {moveParams.PlayerTargeted.User.Name} with swap hands. ";

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

            var keepMyHandCard = moveParams.PlayerTargeted.Cards.FirstOrDefault(y => y.Value == CardValue.KeepMyHand);
            if (keepMyHandCard != null)
            {
                game.LastCardPlayed = new LastCardPlayed(moveParams.TargetedCardColor, keepMyHandCard.Value, keepMyHandCard.ImageUrl, moveParams.PlayerTargeted.User.Name, true);
                moveParams.PlayerTargeted.Cards.Remove(keepMyHandCard);
                game.DiscardedPile.Add(keepMyHandCard);
                messageToLog += $"{moveParams.PlayerTargeted.User.Name} kept their hand safe. ";
            }
            else
            {
                var playersCards = moveParams.PlayerPlayed.Cards.ToList();
                var targetedPlayerCards = moveParams.PlayerTargeted.Cards.ToList();

                moveParams.PlayerPlayed.Cards = targetedPlayerCards;
                moveParams.PlayerTargeted.Cards = playersCards;
                messageToLog += $"Players exchanged hands.";
            }

            messagesToLog.Add(messageToLog);
            return new MoveResult(messagesToLog);
        }
    }
}