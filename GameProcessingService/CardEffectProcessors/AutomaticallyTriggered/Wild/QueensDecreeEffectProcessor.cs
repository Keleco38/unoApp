using System.Linq;
using Common.Enums;
using EntityObjects;
using GameProcessingService.CoreManagers;
using GameProcessingService.Models;

namespace GameProcessingService.CardEffectProcessors.AutomaticallyTriggered.Wild
{
    public class QueensDecreeEffectProcessor : IAutomaticallyTriggeredCardEffectProcessor
    {
        private readonly IGameManager _gameManager;
        public CardValue CardAffected => CardValue.QueensDecree;

        public QueensDecreeEffectProcessor(IGameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public AutomaticallyTriggeredResult ProcessCardEffect(Game game, string messageToLog, AutomaticallyTriggeredParams autoParams)
        {
            var player = autoParams.QueensDecreeParams.PlayerAffected;

            if (game.SilenceTurnsRemaining <= 0)
            {
                if (player.Cards.Count <= 5 && player.Cards.FirstOrDefault(x => x.Value == CardValue.QueensDecree) != null)
                {
                    messageToLog += $"{player.User.Name} had more than 4 cards and queen's decree, auto effect is activated. ";
                    if (game.Players.Count == 2)
                    {
                        var nextPlayer = _gameManager.GetNextPlayer(game, player, game.Players, false);
                        messageToLog += $"Next player ({nextPlayer.User.Name}) will draw a card. ";
                    }
                    else
                    {
                        var previousPlayer = _gameManager.GetNextPlayer(game, player, game.Players, false);
                        var nextPlayer = _gameManager.GetNextPlayer(game, player, game.Players, false);

                        messageToLog += $"Player to the left and right ({previousPlayer.User.Name} and {nextPlayer.User.Name}) will draw a card. ";
                        _gameManager.DrawCard(game, nextPlayer, 1, false);
                        _gameManager.DrawCard(game, previousPlayer, 1, false);
                    }
                }
            }

            return new AutomaticallyTriggeredResult() { MessageToLog = messageToLog };
        }
    }
}