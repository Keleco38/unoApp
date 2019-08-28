using System.Collections.Generic;
using System.Linq;
using Common.Enums;
using EntityObjects;
using GameProcessingService.GameManager;
using GameProcessingService.Models;

namespace GameProcessingService.CardEffectProcessors.Wild
{
    public class DrawFourEffectProcessor:ICardEffectProcessor
    {
        private readonly IGameManager _gameManager;

        public DrawFourEffectProcessor(IGameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {
            var messagesToLog = new List<string>();
            var messageToLog = $"{moveParams.PlayerPlayed.User.Name} targeted {moveParams.PlayerTargeted.User.Name} with +4.";

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

            var numberOfCardsToDraw = 4;

            var doubleDrawCard = moveParams.PlayerTargeted.Cards.FirstOrDefault(c => c.Value == CardValue.DoubleDraw);
            if (doubleDrawCard != null)
            {

                game.LastCardPlayed = new LastCardPlayed(moveParams.TargetedCardColor, doubleDrawCard.Value, doubleDrawCard.ImageUrl, moveParams.PlayerTargeted.User.Name, true);
                moveParams.PlayerTargeted.Cards.Remove(doubleDrawCard);
                game.DiscardedPile.Add(doubleDrawCard);
                numberOfCardsToDraw = numberOfCardsToDraw * 2;
                messageToLog += $"{moveParams.PlayerTargeted.User.Name} doubled the draw effect. ";
            }

            var deflectCard = moveParams.PlayerTargeted.Cards.FirstOrDefault(x => x.Value == CardValue.Deflect);
            if (deflectCard == null)
            {
                messageToLog += $"{moveParams.PlayerTargeted.User.Name} drew {numberOfCardsToDraw} cards.";
                _gameManager.DrawCard(game,moveParams.PlayerTargeted, numberOfCardsToDraw, false);
            }
            else
            {
                game.LastCardPlayed = new LastCardPlayed(moveParams.TargetedCardColor, deflectCard.Value, deflectCard.ImageUrl, moveParams.PlayerTargeted.User.Name, true);
                moveParams.PlayerTargeted.Cards.Remove(deflectCard);
                game.DiscardedPile.Add(deflectCard);
                _gameManager.DrawCard(game,moveParams.PlayerPlayed, numberOfCardsToDraw, false);
                messageToLog += $"{moveParams.PlayerTargeted.User.Name} deflected +4 card. {moveParams.PlayerPlayed.User.Name} must draw {numberOfCardsToDraw} cards.";
            }
            messagesToLog.Add(messageToLog);
            return new MoveResult(messagesToLog);
        }
    }
}