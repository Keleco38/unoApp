using System.Collections.Generic;
using System.Linq;
using Common.Enums;
using EntityObjects;
using EntityObjects.Cards.Abstraction;
using GameProcessingService.CoreManagers.GameManagers;
using GameProcessingService.Models;

namespace GameProcessingService.CardEffectProcessors.Wild
{
    public class ParadigmShiftEffectProcessor:ICardEffectProcessor
    {
        private readonly IGameManager _gameManager;

        public ParadigmShiftEffectProcessor(IGameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {
            var messagesToLog = new List<string>();
            var messageToLog = $"{moveParams.PlayerPlayed.User.Name} played paradigm shift. Every player exchanged their hand with the next player. ";
            List<ICard> firstCardsBackup = null;

            var playersWithKeepMyHandCard = game.Players.Where(x => x.Cards.FirstOrDefault(y => y.Value == CardValue.KeepMyHand) != null).ToList();
            var playersWithOutKeepMyHandCard = game.Players.Where(x => x.Cards.FirstOrDefault(y => y.Value == CardValue.KeepMyHand) == null).ToList();

            playersWithKeepMyHandCard.ForEach(p =>
            {
                var keepMyHandCard = p.Cards.FirstOrDefault(y => y.Value == CardValue.KeepMyHand);
                game.LastCardPlayed = new LastCardPlayed(moveParams.TargetedCardColor, keepMyHandCard.Value, keepMyHandCard.ImageUrl, p.User.Name, true);
                p.Cards.Remove(keepMyHandCard);
                game.DiscardedPile.Add(keepMyHandCard);
                messageToLog += $"{p.User.Name} kept their hand safe. ";
            });

            Player loopingPlayer = null;

            for (int i = 0; i < playersWithOutKeepMyHandCard.Count; i++)
            {
                if (i == 0)
                {
                    loopingPlayer = playersWithOutKeepMyHandCard[0];
                    firstCardsBackup = loopingPlayer.Cards.ToList();
                }

                if (i != playersWithOutKeepMyHandCard.Count - 1)
                {
                    loopingPlayer.Cards = _gameManager.GetNextPlayer(game,loopingPlayer, playersWithOutKeepMyHandCard).Cards;
                }
                else
                {
                    loopingPlayer.Cards = firstCardsBackup;
                }
                loopingPlayer = _gameManager.GetNextPlayer(game,loopingPlayer, playersWithOutKeepMyHandCard);
            }

            messagesToLog.Add(messageToLog);
            return new MoveResult(messagesToLog);
        }
    }
}