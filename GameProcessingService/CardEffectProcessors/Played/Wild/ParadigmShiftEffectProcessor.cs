using System.Collections.Generic;
using System.Linq;
using Common.Enums;
using EntityObjects;
using EntityObjects.Cards.Abstraction;
using GameProcessingService.CardEffectProcessors.AutomaticallyTriggered;
using GameProcessingService.CoreManagers;
using GameProcessingService.Models;

namespace GameProcessingService.CardEffectProcessors.Played.Wild
{
    public class ParadigmShiftEffectProcessor : IPlayedCardEffectProcessor
    {
        private readonly IGameManager _gameManager;
        private readonly IEnumerable<IAutomaticallyTriggeredCardEffectProcessor> _automaticallyTriggeredCardEffectProcessors;
        public CardValue CardAffected => CardValue.ParadigmShift;

        public ParadigmShiftEffectProcessor(IGameManager gameManager, IEnumerable<IAutomaticallyTriggeredCardEffectProcessor> automaticallyTriggeredCardEffectProcessors)
        {
            _gameManager = gameManager;
            _automaticallyTriggeredCardEffectProcessors = automaticallyTriggeredCardEffectProcessors;
        }

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams, string messageToLog)
        {
            messageToLog += $"{moveParams.PlayerPlayed.User.Name} played paradigm shift. Every player exchanged their hand with the next player. ";

            var automaticallyTriggeredResultKeepMyHand = _automaticallyTriggeredCardEffectProcessors.First(x => x.CardAffected == CardValue.KeepMyHand).ProcessCardEffect(game, messageToLog, new AutomaticallyTriggeredParams() { KeepMyHandParams = new AutomaticallyTriggeredKeepMyHandParams(game.Players, moveParams.TargetedCardColor) });
            messageToLog = automaticallyTriggeredResultKeepMyHand.MessageToLog;

            Player loopingPlayer = null;
            List<ICard> firstCardsBackup = null;

            for (int i = 0; i < automaticallyTriggeredResultKeepMyHand.PlayersWithoutKeepMyHand.Count; i++)
            {
                if (i == 0)
                {
                    loopingPlayer = automaticallyTriggeredResultKeepMyHand.PlayersWithoutKeepMyHand[0];
                    firstCardsBackup = loopingPlayer.Cards.ToList();
                }

                if (i != automaticallyTriggeredResultKeepMyHand.PlayersWithoutKeepMyHand.Count - 1)
                {
                    loopingPlayer.Cards = _gameManager.GetNextPlayer(game, loopingPlayer, automaticallyTriggeredResultKeepMyHand.PlayersWithoutKeepMyHand).Cards;
                }
                else
                {
                    loopingPlayer.Cards = firstCardsBackup;
                }
                loopingPlayer = _gameManager.GetNextPlayer(game, loopingPlayer, automaticallyTriggeredResultKeepMyHand.PlayersWithoutKeepMyHand);
            }

            return new MoveResult(messageToLog);
        }
    }
}