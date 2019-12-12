using System.Collections.Generic;
using System.Linq;
using Common.Enums;
using EntityObjects;
using GameProcessingService.CardEffectProcessors.AutomaticallyTriggered;
using GameProcessingService.CoreManagers;
using GameProcessingService.Models;

namespace GameProcessingService.CardEffectProcessors.Played.Wild
{
    public class FreshStartEffectProcessor : IPlayedCardEffectProcessor
    {
        private readonly IGameManager _gameManager;
        private readonly IEnumerable<IAutomaticallyTriggeredCardEffectProcessor> _automaticallyTriggeredCardEffectProcessors;
        public CardValue CardAffected => CardValue.FreshStart;

        public FreshStartEffectProcessor(IGameManager gameManager, IEnumerable<IAutomaticallyTriggeredCardEffectProcessor> automaticallyTriggeredCardEffectProcessors)
        {
            _gameManager = gameManager;
            _automaticallyTriggeredCardEffectProcessors = automaticallyTriggeredCardEffectProcessors;
        }

        public MoveResult ProcessCardEffect(Game game, MoveParams moveParams)
        {
            var messagesToLog = new List<string>();
            var messageToLog = $"{moveParams.PlayerPlayed.User.Name} played Fresh start. ";

            if (moveParams.ActivateSpecialCardEffect)
            {
                messageToLog += "Player activated the special effect. ";

                var automaticallyTriggeredResultKeepMyHand = _automaticallyTriggeredCardEffectProcessors.First(x => x.CardAffected == CardValue.KeepMyHand).ProcessCardEffect(game, messageToLog, new AutomaticallyTriggeredParams() { KeepMyHandParams = new AutomaticallyTriggeredKeepMyHandParams(new List<Player>(){moveParams.PlayerPlayed}, moveParams.TargetedCardColor) });
                messageToLog = automaticallyTriggeredResultKeepMyHand.MessageToLog;

                automaticallyTriggeredResultKeepMyHand.PlayersWithoutKeepMyHand.ForEach(p =>
                {
                    game.DiscardedPile.AddRange(p.Cards.ToList());
                    p.Cards.Clear();
                    _gameManager.DrawCard(game, p, 7, false);
                });

            }
            else
            {
                messageToLog += "Player did not activate the special effect of the card. They will draw 1 card.";
                _gameManager.DrawCard(game, moveParams.PlayerPlayed, 1, false);
            }



            messagesToLog.Add(messageToLog);
            return new MoveResult(messagesToLog);
        }
    }
}