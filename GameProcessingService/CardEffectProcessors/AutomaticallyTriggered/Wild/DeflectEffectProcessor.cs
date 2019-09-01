using System.Linq;
using Common.Enums;
using EntityObjects;
using GameProcessingService.CoreManagers;
using GameProcessingService.Models;

namespace GameProcessingService.CardEffectProcessors.AutomaticallyTriggered.Wild
{
    public class DeflectEffectProcessor: IAutomaticallyTriggeredCardEffectProcessor
    {
        private readonly IGameManager _gameManager;
        public CardValue CardAffected => CardValue.Deflect;

        public DeflectEffectProcessor(IGameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public AutomaticallyTriggeredResult ProcessCardEffect(Game game, AutomaticallyTriggeredParams automaticallyTriggeredParams)
        {
            if (automaticallyTriggeredParams.MoveParams.PlayerPlayed == automaticallyTriggeredParams.MoveParams.PlayerTargeted)
            {
                automaticallyTriggeredParams.MessageToLog += $"{automaticallyTriggeredParams.MoveParams.PlayerTargeted.User.Name} drew {automaticallyTriggeredParams.NumberOfCardsToDraw} cards. ";
                _gameManager.DrawCard(game, automaticallyTriggeredParams.MoveParams.PlayerTargeted, automaticallyTriggeredParams.NumberOfCardsToDraw, false);

            }
            else
            {
                var deflectCard = automaticallyTriggeredParams.MoveParams.PlayerTargeted.Cards.FirstOrDefault(x => x.Value == CardValue.Deflect);
                if (deflectCard == null)
                {
                    automaticallyTriggeredParams.MessageToLog += $"{automaticallyTriggeredParams.MoveParams.PlayerTargeted.User.Name} drew {automaticallyTriggeredParams.NumberOfCardsToDraw} cards. ";
                    _gameManager.DrawCard(game, automaticallyTriggeredParams.MoveParams.PlayerTargeted, automaticallyTriggeredParams.NumberOfCardsToDraw, false);
                }
                else
                {
                    game.LastCardPlayed = new LastCardPlayed(automaticallyTriggeredParams.MoveParams.TargetedCardColor, deflectCard.Value, deflectCard.ImageUrl, automaticallyTriggeredParams.MoveParams.PlayerTargeted.User.Name, true);
                    automaticallyTriggeredParams.MoveParams.PlayerTargeted.Cards.Remove(deflectCard);
                    game.DiscardedPile.Add(deflectCard);
                    _gameManager.DrawCard(game, automaticallyTriggeredParams.MoveParams.PlayerPlayed, automaticallyTriggeredParams.NumberOfCardsToDraw, false);
                    automaticallyTriggeredParams.MessageToLog += $"{automaticallyTriggeredParams.MoveParams.PlayerTargeted.User.Name} deflected {automaticallyTriggeredParams.MoveParams.CardPlayed.Value.ToString()}. {automaticallyTriggeredParams.MoveParams.PlayerPlayed.User.Name} must draw {automaticallyTriggeredParams.NumberOfCardsToDraw} cards.";
                }
            }


            return new AutomaticallyTriggeredResult(automaticallyTriggeredParams.MessageToLog);
        }
    }
}