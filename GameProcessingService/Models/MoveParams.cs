using System.Collections.Generic;
using Common.Enums;
using EntityObjects;
using EntityObjects.Cards.Abstraction;

namespace GameProcessingService.Models
{
    public class MoveParams
    {
        public MoveParams(Player playerPlayed, ICard cardPlayed, Player playerTargeted, CardColor targetedCardColor, ICard cardToDig, List<int> duelNumbers, List<ICard> charityCards, int blackjackNumber,
            List<int> numbersToDiscard, ICard cardPromisedToDiscard, string oddOrEvenGuess, LastCardPlayed previousLastCardPlayed, CardValue targetedCardValue, bool activateSpecialCardEffect)
        {
            PlayerPlayed = playerPlayed;
            CardPlayed = cardPlayed;
            PlayerTargeted = playerTargeted;
            TargetedCardColor = targetedCardColor;
            CardToDig = cardToDig;
            DuelNumbers = duelNumbers;
            CharityCards = charityCards;
            BlackjackNumber = blackjackNumber;
            NumbersToDiscard = numbersToDiscard;
            CardPromisedToDiscard = cardPromisedToDiscard;
            OddOrEvenGuess = oddOrEvenGuess;
            PreviousLastCardPlayed = previousLastCardPlayed;
            TargetedCardValue = targetedCardValue;
            ActivateSpecialCardEffect = activateSpecialCardEffect;
        }

        public Player PlayerPlayed { get; set; }
        public Player PlayerTargeted { get; set; }
        public ICard CardPlayed { get; set; }
        public CardColor TargetedCardColor { get; set; }
        public ICard CardToDig { get; set; }
        public ICard CardPromisedToDiscard { get; set; }
        public List<int> DuelNumbers { get; set; }
        public List<ICard> CharityCards { get; set; }
        public int BlackjackNumber { get; set; }
        public string OddOrEvenGuess { get; set; }
        public LastCardPlayed PreviousLastCardPlayed { get; }
        public CardValue TargetedCardValue { get; }
        public bool ActivateSpecialCardEffect { get; }
        public List<int> NumbersToDiscard { get; set; }

    }
}