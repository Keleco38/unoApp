using Common.Enums;
using EntityObjects;
using GameProcessingService.Models;

namespace GameProcessingService.CardEffectProcessors.Played
{
    public interface IPlayedCardEffectProcessor
    {
        CardValue CardAffected { get; }
        MoveResult ProcessCardEffect(Game game, MoveParams moveParams, string messageToLog);
    }
}