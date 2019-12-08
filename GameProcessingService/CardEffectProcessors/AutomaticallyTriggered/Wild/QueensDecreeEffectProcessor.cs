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
                    messageToLog += $"{player.User.Name} had less than 5 cards and queen's decree, auto effect is activated. ";
                    if (game.Players.Count == 2)
                    {
                        var nextPlayer = _gameManager.GetNextPlayer(game, player, game.Players, false);
                        messageToLog += $"Next player ({nextPlayer.User.Name}) will draw a card. ";


                        var kingsDecreeResult = BlockedByKingsDecree(messageToLog, nextPlayer, game);
                        messageToLog = kingsDecreeResult.MessageToLog;
                        if (!kingsDecreeResult.ActivatedKingsDecree)
                        {
                            _gameManager.DrawCard(game, nextPlayer, 1, false);
                        }
                    }
                    else
                    {
                        var previousPlayer = _gameManager.GetNextPlayer(game, player, game.Players, false);
                        var nextPlayer = _gameManager.GetNextPlayer(game, player, game.Players, false);

                        messageToLog += $"Player to the left and right ({previousPlayer.User.Name} and {nextPlayer.User.Name}) will draw a card. ";

                        var kingsDecreeResult = BlockedByKingsDecree(messageToLog, nextPlayer, game);
                        messageToLog = kingsDecreeResult.MessageToLog;
                        if (!kingsDecreeResult.ActivatedKingsDecree)
                        {
                            _gameManager.DrawCard(game, nextPlayer, 1, false);
                        }

                        kingsDecreeResult = BlockedByKingsDecree(messageToLog, previousPlayer, game);
                        messageToLog = kingsDecreeResult.MessageToLog;
                        if (!kingsDecreeResult.ActivatedKingsDecree)
                        {
                            _gameManager.DrawCard(game, previousPlayer, 1, false);
                        }


                    }
                }
            }

            return new AutomaticallyTriggeredResult() { MessageToLog = messageToLog };
        }

        private AutomaticallyTriggeredResult BlockedByKingsDecree(string messageToLog, Player player, Game game)
        {
            var activatedKingsDecree = false;

            if (game.SilenceTurnsRemaining <= 0)
            {
                if (player.Cards.Count > 4 && player.Cards.FirstOrDefault(x => x.Value == CardValue.KingsDecree) != null)
                {
                    activatedKingsDecree = true;
                    messageToLog += $"{player.User.Name} is not affected by the draw. He has more than 4 cards and king's decree in hand (auto effect is activated).";
                }
            }

            return new AutomaticallyTriggeredResult() { MessageToLog = messageToLog, ActivatedKingsDecree = activatedKingsDecree };
        }
    }
}