using System.Collections.Generic;
using Uno.Enums;
using Uno.Models.Entities;
using Uno.Models.Entities.Cards.Abstraction;

namespace Uno.Models.Helpers
{
    public class MoveParams
    {
        public MoveParams(Player playerPlayed, Player playerTargeted, CardColor targetedCardColor, ICard cardToDig, List<int> duelNumbers, List<ICard> charityCards, int blackjackNumber, List<int> numbersToDiscard)
        {
            PlayerPlayed = playerPlayed;
            PlayerTargeted = playerTargeted;
            TargetedCardColor = targetedCardColor;
            CardToDig = cardToDig;
            DuelNumbers = duelNumbers;
            CharityCards = charityCards;
            BlackjackNumber = blackjackNumber;
            NumbersToDiscard = numbersToDiscard;
        }

        public Player PlayerPlayed { get; set; }
        public Player PlayerTargeted { get; set; }
        public CardColor TargetedCardColor { get; set; }
        public ICard CardToDig { get; set; }
        public List<int> DuelNumbers { get; set; }
        public List<ICard> CharityCards { get; set; }
        public int BlackjackNumber { get; set; }
        public List<int> NumbersToDiscard { get; set; }

    }
}