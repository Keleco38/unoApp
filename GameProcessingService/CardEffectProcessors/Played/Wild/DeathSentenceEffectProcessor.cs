using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Common.Enums;
using EntityObjects;
using GameProcessingService.CoreManagers;
using GameProcessingService.Models;

namespace GameProcessingService.CardEffectProcessors.Played.Wild
{
    public class DeathSentenceEffectProcessor : IPlayedCardEffectProcessor
    {
        private readonly IGameManager _gameManager;
        public CardValue CardAffected => CardValue.DeathSentence;

        public DeathSentenceEffectProcessor(IGameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {
            var messagesToLog = new List<string>();
            var messageToLog = $"{moveParams.PlayerPlayed.User.Name} played DeathSentence. ";

            messageToLog += $"{moveParams.TargetedCardValue.ToString()} is removed from the game.";
            game.Players.ForEach(player => player.Cards.RemoveAll(x => x.Value == moveParams.TargetedCardValue));
            game.Deck.Cards.RemoveAll(x => x.Value == moveParams.TargetedCardValue);
            game.DiscardedPile.RemoveAll(x => x.Value == moveParams.TargetedCardValue);
            if (!game.CardValuesRemovedFromTheRound.Contains(moveParams.TargetedCardValue))
            {
                game.CardValuesRemovedFromTheRound.Add(moveParams.TargetedCardValue);
            }

            messagesToLog.Add(messageToLog);
            return new MoveResult(messagesToLog);
        }
    }
}