using System.Collections.Generic;
using Common.Enums;
using System.Linq;
using EntityObjects;
using GameProcessingService.CoreManagers;
using GameProcessingService.Models;

namespace GameProcessingService.CardEffectProcessors.Wild
{
    public class CharityEffectProcessor : ICardEffectProcessor
    {

        private readonly IGameManager _gameManager;
        public CardValue CardAffected => CardValue.Charity;

        public CharityEffectProcessor(IGameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {
            var messagesToLog = new List<string>();

            var messageToLog = $"{moveParams.PlayerPlayed.User.Name} targeted {moveParams.PlayerTargeted.User.Name} with Charity. ";

            Player loopingPlayer = _gameManager.GetNextPlayer(game, moveParams.PlayerPlayed, game.Players);
            var playerExcludingPlayerPlaying = game.Players.Where(p => p != moveParams.PlayerPlayed).ToList();
            for (int i = 0; i < playerExcludingPlayerPlaying.Count; i++)
            {
                if (i != 0)
                {
                    loopingPlayer = _gameManager.GetNextPlayer(game, loopingPlayer, playerExcludingPlayerPlaying);
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

            var charityCardsString = string.Empty;
            moveParams.CharityCards.ForEach(x =>
            {
                charityCardsString += x.Color + " " + x.Value + ", ";
                moveParams.PlayerPlayed.Cards.Remove(moveParams.PlayerPlayed.Cards.First(c => c.Value == x.Value && c.Color == x.Color));
                moveParams.PlayerTargeted.Cards.Add(x);
            });
            messageToLog += $" Player gave them two cards: {charityCardsString}";

            _gameManager.DrawCard(game, moveParams.PlayerPlayed, 1, false);

            messagesToLog.Add(messageToLog);
            return new MoveResult(messagesToLog);
        }
    }
}