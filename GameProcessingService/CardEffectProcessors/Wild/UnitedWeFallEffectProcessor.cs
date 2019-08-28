using System.Collections.Generic;
using System.Linq;
using Common.Enums;
using EntityObjects;
using GameProcessingService.CoreManagers.GameManagers;
using GameProcessingService.Models;

namespace GameProcessingService.CardEffectProcessors.Wild
{
    public class UnitedWeFallEffectProcessor:ICardEffectProcessor
    {
        private readonly IGameManager _gameManager;

        public UnitedWeFallEffectProcessor(IGameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {
            var messagesToLog = new List<string>();
            var messageToLog = $"{moveParams.PlayerPlayed.User.Name}  played united we fall. Every player drew 2 cards. ";

            game.Players.ForEach(x =>
            {
                var doubleDrawCard = x.Cards.FirstOrDefault(c => c.Value == CardValue.DoubleDraw);
                if (doubleDrawCard != null)
                {
                    _gameManager.DrawCard(game,x, 4, false);

                    game.LastCardPlayed = new LastCardPlayed(moveParams.TargetedCardColor, doubleDrawCard.Value, doubleDrawCard.ImageUrl, x.User.Name, true);
                    x.Cards.Remove(doubleDrawCard);
                    game.DiscardedPile.Add(doubleDrawCard);

                    messageToLog += $"{x.User.Name}  drew double.";
                }
                else
                {
                    _gameManager.DrawCard(game,x, 2, false);
                }
            });
            messagesToLog.Add(messageToLog);
            return new MoveResult(messagesToLog);
        }
    }
}