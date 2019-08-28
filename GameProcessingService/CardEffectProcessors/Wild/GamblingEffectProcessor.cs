using System.Collections.Generic;
using System.Linq;
using Common.Enums;
using EntityObjects;
using GameProcessingService.CoreManagers.GameManagers;
using GameProcessingService.Models;

namespace GameProcessingService.CardEffectProcessors.Wild
{
    public class GamblingEffectProcessor:ICardEffectProcessor
    {
        private readonly IGameManager _gameManager;

        public GamblingEffectProcessor(IGameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {
            var messagesToLog = new List<string>();
            var messageToLog = $"{moveParams.PlayerPlayed.User.Name} targeted {moveParams.PlayerTargeted.User.Name} with Gambling. ";

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

            var correctGuess = moveParams.PlayerTargeted.Cards.Count(x => ((int)x.Value < 10)) % 2 == 0 ? "even" : "odd";

            if (correctGuess == moveParams.OddOrEvenGuess)
            {
                messageToLog += $"Player guessed correctly. {moveParams.PlayerTargeted.User.Name} had {correctGuess} number of numbered (0-9) cards. They will discard 1 card";
                if (moveParams.PlayerPlayed.Cards.Count > 0)
                {
                    moveParams.PlayerPlayed.Cards.RemoveRange(0, 1);
                }
            }
            else
            {
                var numberOfCardsToDraw = 3;

                var doubleDrawCard = moveParams.PlayerPlayed.Cards.FirstOrDefault(c => c.Value == CardValue.DoubleDraw);
                if (doubleDrawCard != null)
                {
                    game.LastCardPlayed = new LastCardPlayed(moveParams.TargetedCardColor, doubleDrawCard.Value, doubleDrawCard.ImageUrl, moveParams.PlayerPlayed.User.Name, true);
                    moveParams.PlayerPlayed.Cards.Remove(doubleDrawCard);
                    game.DiscardedPile.Add(doubleDrawCard);
                    numberOfCardsToDraw = numberOfCardsToDraw * 2;
                    messageToLog += $"{moveParams.PlayerPlayed.User.Name} doubled the draw effect. ";
                }

                messageToLog += $"Player guessed wrongly. {moveParams.PlayerTargeted.User.Name} had {correctGuess} number of numbered (0-9) cards. They will draw {numberOfCardsToDraw} cards";
                _gameManager.DrawCard(game,moveParams.PlayerPlayed, numberOfCardsToDraw, false);
                //double draw
            }

            messagesToLog.Add(messageToLog);
            return new MoveResult(messagesToLog);
        }
    }
}