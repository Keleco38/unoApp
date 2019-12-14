using System.Collections.Generic;
using System.Linq;
using Common.Enums;
using EntityObjects;
using GameProcessingService.CoreManagers;
using GameProcessingService.Models;

namespace GameProcessingService.CardEffectProcessors.Played.Wild
{
    public class SummonWildcardEffectProcessor : IPlayedCardEffectProcessor
    {
        private readonly IGameManager _gameManager;
        public CardValue CardAffected => CardValue.SummonWildcard;

        public SummonWildcardEffectProcessor(IGameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {
            var messagesToLog = new List<string>();
            var messageToLog = $"{moveParams.PlayerPlayed.User.Name} played SummonWildcard. ";



            if (moveParams.ActivateSpecialCardEffect)
            {
                messageToLog += $"{moveParams.PlayerPlayed.User.Name} activated effect, drew 3 cards, summoned {moveParams.TargetedCardValue.ToString()} to their hand and kept their turn. ";
                var card = game.Deck.Cards.FirstOrDefault(x => x.Value == moveParams.TargetedCardValue);
                if (card != null)
                {
                    moveParams.PlayerPlayed.Cards.Add(card);
                    game.Deck.Cards.Remove(card);
                }
                else
                {
                    //if there is nothing from the deck, take one from the discarded pile
                    card = game.DiscardedPile.FirstOrDefault(x => x.Value == moveParams.TargetedCardValue);
                    if (card != null)
                    {
                        moveParams.PlayerPlayed.Cards.Add(card);
                        game.DiscardedPile.Remove(card);
                    }
                    else
                    {
                        messageToLog += "Summoning failed. That card does not exists in the deck or the discarded pile.";
                    }
                }

                _gameManager.DrawCard(game, moveParams.PlayerPlayed, 3, false);
            }
            else
            {
                messageToLog += $"{moveParams.PlayerPlayed.User.Name} did not activate the effect of the card. They will draw 1 card and keep their turn. ";
                _gameManager.DrawCard(game, moveParams.PlayerPlayed, 1, false);
       
            }

            var previousPlayer = _gameManager.GetNextPlayer(game, moveParams.PlayerPlayed, game.Players, true);
            game.PlayerToPlay = previousPlayer;


            messagesToLog.Add(messageToLog);
            return new MoveResult(messagesToLog);
        }
    }
}