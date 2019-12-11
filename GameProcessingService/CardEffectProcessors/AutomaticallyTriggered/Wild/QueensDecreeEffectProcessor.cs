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
                    if (game.Players.Count == 2)
                    {
                        var nextPlayer = _gameManager.GetNextPlayer(game, player, game.Players, false);

                        if (nextPlayer.Cards.Count < player.Cards.Count)
                        {
                            messageToLog += $"{player.User.Name} auto activated queen's decree. ";
                            messageToLog += $"Next player ({nextPlayer.User.Name}) will draw a card. ";
                            var kingsDecreeResult = BlockedByKingsDecree(messageToLog, nextPlayer, game);
                            messageToLog = kingsDecreeResult.MessageToLog;
                            if (!kingsDecreeResult.ActivatedKingsDecree)
                            {
                                _gameManager.DrawCard(game, nextPlayer, 1, false);
                            }
                        }
                    }
                    else
                    {
                        var previousPlayer = _gameManager.GetNextPlayer(game, player, game.Players, true);
                        var nextPlayer = _gameManager.GetNextPlayer(game, player, game.Players, false);

                        if (nextPlayer.Cards.Count < player.Cards.Count || previousPlayer.Cards.Count < player.Cards.Count)
                        {
                            messageToLog += $"{player.User.Name} auto activated queen's decree. ";
                            if (nextPlayer.Cards.Count < player.Cards.Count)
                            {
                                messageToLog += $"Next player ({nextPlayer.User.Name}) will draw a card. ";
                                var kingsDecreeResult = BlockedByKingsDecree(messageToLog, nextPlayer, game);
                                messageToLog = kingsDecreeResult.MessageToLog;
                                if (!kingsDecreeResult.ActivatedKingsDecree)
                                {
                                    _gameManager.DrawCard(game, nextPlayer, 1, false);
                                }
                            }

                            if (previousPlayer.Cards.Count < player.Cards.Count)
                            {
                                messageToLog += $"Previous player player ({previousPlayer.User.Name}) will draw a card. ";
                                var kingsDecreeResult = BlockedByKingsDecree(messageToLog, previousPlayer, game);
                                messageToLog = kingsDecreeResult.MessageToLog;
                                if (!kingsDecreeResult.ActivatedKingsDecree)
                                {
                                    _gameManager.DrawCard(game, previousPlayer, 1, false);
                                }
                            }
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
                    messageToLog += $"{player.User.Name} is not affected by the draw (king's decree).";
                }
            }

            return new AutomaticallyTriggeredResult() { MessageToLog = messageToLog, ActivatedKingsDecree = activatedKingsDecree };
        }
    }
}