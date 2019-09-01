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
    public class SwapHandsEffectProcessor : IPlayedCardEffectProcessor
    {
        private readonly IGameManager _gameManager;
        private readonly IEnumerable<IAutomaticallyTriggeredCardEffectProcessor> _automaticallyTriggeredCardEffectProcessors;
        public CardValue CardAffected => CardValue.SwapHands;

        public SwapHandsEffectProcessor(IGameManager gameManager, IEnumerable<IAutomaticallyTriggeredCardEffectProcessor> automaticallyTriggeredCardEffectProcessors)
        {
            _gameManager = gameManager;
            _automaticallyTriggeredCardEffectProcessors = automaticallyTriggeredCardEffectProcessors;
        }

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {
            var messagesToLog = new List<string>();
            var messageToLog = $"{moveParams.PlayerPlayed.User.Name} targeted {moveParams.PlayerTargeted.User.Name} with swap hands. ";

            var automaticallyTriggeredResultMagneticPolarity = _automaticallyTriggeredCardEffectProcessors.First(x => x.CardAffected == CardValue.MagneticPolarity).ProcessCardEffect(game, new AutomaticallyTriggeredParams(moveParams, messageToLog, null, 0));
            messageToLog = automaticallyTriggeredResultMagneticPolarity.MessageToLog;

            var automaticallyTriggeredResultKeepMyHand = _automaticallyTriggeredCardEffectProcessors.First(x => x.CardAffected == CardValue.KeepMyHand).ProcessCardEffect(game, new AutomaticallyTriggeredParams(moveParams, messageToLog, game.Players, 0));
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

            messagesToLog.Add(messageToLog);
            return new MoveResult(messagesToLog);
        }
    }
}