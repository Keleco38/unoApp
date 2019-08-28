using System.Collections.Generic;
using System.Linq;
using EntityObjects;
using GameProcessingService.GameManager;
using GameProcessingService.Models;

namespace GameProcessingService.CardEffectProcessors.Wild
{
    public class DiscardNumberEffectProcessor:ICardEffectProcessor
    {

        private readonly IGameManager _gameManager;

        public DiscardNumberEffectProcessor(IGameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {
            var messagesToLog = new List<string>();
            var messageToLog = $"{moveParams.PlayerPlayed.User.Name}  played discard number. Numbers that are discarded: {string.Join(' ', moveParams.NumbersToDiscard)}. ";
            game.Players.ForEach(p =>
            {
                var cardsToDiscard = p.Cards.Where(c => moveParams.NumbersToDiscard.Contains((int)c.Value)).ToList();
                cardsToDiscard.ForEach(x => p.Cards.Remove(x));
            });
            _gameManager.DrawCard(game,moveParams.PlayerPlayed, 1, false);
            messagesToLog.Add(messageToLog);
            return new MoveResult(messagesToLog);
        }
    }
}