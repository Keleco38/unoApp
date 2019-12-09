using System.Collections.Generic;
using System.Linq;
using Common.Enums;
using EntityObjects;
using GameProcessingService.CoreManagers;
using GameProcessingService.Models;

namespace GameProcessingService.CardEffectProcessors.Played.Wild
{
    public class DevilsDealEffectProcessor : IPlayedCardEffectProcessor
    {
        private readonly IGameManager _gameManager;
        public CardValue CardAffected => CardValue.DevilsDeal;

        public DevilsDealEffectProcessor(IGameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {
            var messagesToLog = new List<string>();
            var messageToLog = $"{moveParams.PlayerPlayed.User.Name} played Devil's Deal.";

            if (moveParams.ActivateSpecialCardEffect)
            {
                messageToLog += $"{moveParams.PlayerPlayed.User.Name} activated the special effect. they will draw 2 cards and every player that has 1 card will draw 1 card. This card has been returned to the owner's hands";
                var playersAffected = game.Players.Where(x => x.Cards.Count == 1);
                foreach (var player in playersAffected)
                {
                    _gameManager.DrawCard(game, player, 1, false);
                }
                _gameManager.DrawCard(game, moveParams.PlayerPlayed, 2, false);
                game.DiscardedPile.Remove(moveParams.CardPlayed);
                moveParams.PlayerPlayed.Cards.Add(moveParams.CardPlayed);
            }
            else
            {
                messageToLog += $"{moveParams.PlayerPlayed.User.Name} did not activate the special effect of the card.";
            }

            messagesToLog.Add(messageToLog);
            return new MoveResult(messagesToLog);
        }
    }
}