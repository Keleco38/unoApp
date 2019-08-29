using Common.Enums;
using EntityObjects;
using GameProcessingService.Models;

namespace GameProcessingService.CardEffectProcessors
{
    public interface ICardEffectProcessor
    {
        CardValue CardAffected { get;  }
        MoveResult ProcessCardEffect(Game game, MoveParams moveParams);
    }
}