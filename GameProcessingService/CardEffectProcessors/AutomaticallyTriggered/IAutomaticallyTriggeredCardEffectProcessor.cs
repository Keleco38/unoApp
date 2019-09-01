using Common.Enums;
using EntityObjects;
using GameProcessingService.Models;

namespace GameProcessingService.CardEffectProcessors.AutomaticallyTriggered
{
    public interface IAutomaticallyTriggeredCardEffectProcessor
    {
        CardValue CardAffected { get; }
        AutomaticallyTriggeredResult ProcessCardEffect(Game game, AutomaticallyTriggeredParams automaticallyTriggeredParams);
    }
}