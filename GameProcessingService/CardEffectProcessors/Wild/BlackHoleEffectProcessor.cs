using System.Collections.Generic;
using System.Linq;
using Common.Enums;
using EntityObjects;
using GameProcessingService.CoreManagers;
using GameProcessingService.Models;

namespace GameProcessingService.CardEffectProcessors.Wild
{
    public class BlackHoleEffectProcessor : ICardEffectProcessor
    {
        private readonly IGameManager _gameManager;
        public CardValue CardAffected => CardValue.BlackHole;

        public BlackHoleEffectProcessor(IGameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {
            var messagesToLog = new List<string>();
            var messageToLog = $"{moveParams.PlayerPlayed.User.Name} played black hole. Every player drew new hand. ";
            game.Players.ForEach(p =>
            {
                var keepMyHandCard = p.Cards.FirstOrDefault(y => y.Value == CardValue.KeepMyHand);
                if (keepMyHandCard != null)
                {
                    game.LastCardPlayed = new LastCardPlayed(moveParams.TargetedCardColor, keepMyHandCard.Value, keepMyHandCard.ImageUrl, moveParams.PlayerTargeted.User.Name, true);
                    p.Cards.Remove(keepMyHandCard);
                    game.DiscardedPile.Add(keepMyHandCard);
                    messageToLog += $"{p.User.Name} kept their hand safe. ";
                }
                else
                {
                    var cardCount = p.Cards.Count;
                    game.DiscardedPile.AddRange(p.Cards.ToList());
                    p.Cards.Clear();
                    _gameManager.DrawCard(game, p, cardCount, false);
                }

            });
            messagesToLog.Add(messageToLog);
            return new MoveResult(messagesToLog);
        }
    }
}