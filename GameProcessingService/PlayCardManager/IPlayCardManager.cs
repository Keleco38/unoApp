using System.Collections.Generic;
using Common.Enums;
using EntityObjects;
using GameProcessingService.Models;

namespace GameProcessingService.PlayCardManager
{
    public interface IPlayCardManager
    {
        MoveResult PlayCard(Game game, Player playerPlayed, string cardPlayedId, CardColor targetedCardColor, string playerTargetedId, string cardToDigId, List<int> duelNumbers,
            List<string> charityCardsIds, int blackjackNumber, List<int> numbersToDiscard, string cardPromisedToDiscardId, string oddOrEvenGuess);
    }
}