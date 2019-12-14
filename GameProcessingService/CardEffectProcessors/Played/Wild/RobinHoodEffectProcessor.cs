using System;
using System.Collections.Generic;
using System.Linq;
using Common.Enums;
using Common.Extensions;
using EntityObjects;
using GameProcessingService.CoreManagers;
using GameProcessingService.Models;

namespace GameProcessingService.CardEffectProcessors.Played.Wild
{
    public class RobinHoodEffectProcessor : IPlayedCardEffectProcessor
    {
        private readonly IGameManager _gameManager;
        public CardValue CardAffected => CardValue.RobinHood;

        public RobinHoodEffectProcessor(IGameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {
            var messagesToLog = new List<string>();
            var playersWithMostCards = game.Players.Where(y => y.Cards.Count == game.Players.Max(p => p.Cards.Count)).ToList();
            var playersWIthLeastCards = game.Players.Where(y => y.Cards.Count == game.Players.Min(p => p.Cards.Count)).ToList();


            Random rng=new Random();
            var playerWithMostCards = playersWithMostCards[rng.Next(playersWithMostCards.Count)];
            var playerWIthLeastCards = playersWIthLeastCards[rng.Next(playersWIthLeastCards.Count)];

            var maxNoToTake = playerWithMostCards.Cards.Count < 5 ? 1 : 2;

            var numberOfCardsToTake = playerWithMostCards.Cards.Count < maxNoToTake ? playerWithMostCards.Cards.Count : maxNoToTake;

            var cards = playerWithMostCards.Cards.GetAndRemove(0, numberOfCardsToTake);

            var messageToLog = $"{moveParams.PlayerPlayed.User.Name} played RobinHood. They stole {string.Join(", ", cards.Select(x=>$"{x.Color.ToString()} {x.Value.ToString()}"))} cards from {playerWithMostCards.User.Name}, and gave them to {playerWIthLeastCards.User.Name}. ";

            playerWIthLeastCards.Cards.AddRange(cards);

            messagesToLog.Add(messageToLog);
            return new MoveResult(messagesToLog);
        }
    }
}