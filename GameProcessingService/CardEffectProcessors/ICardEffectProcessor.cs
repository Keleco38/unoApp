using EntityObjects;
using GameProcessingService.Models;

namespace GameProcessingService.CardEffectProcessors
{
    public interface ICardEffectProcessor
    {
        MoveResult ProcessCardEffect(Game game, MoveParams moveParams);
    }
}