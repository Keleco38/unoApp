using System;
using System.Collections.Generic;
using System.Linq;
using Uno.Enums;
using Uno.Models.Entities.Cards.Abstraction;
using Uno.Models.Entities.Cards.Colored;
using Uno.Models.Entities.Cards.Wild;

namespace Uno.Models.Entities
{
    public class Deck
    {
        public Deck(List<CardValue> bannedCards)
        {
            InitializeCards(bannedCards);
            Shuffle();
        }

        public List<ICard> Cards { get; set; }


        public List<ICard> Draw(int count)
        {
            var cardsDrew = Cards.Take(count).ToList();

            //Remove the drawn Deck from the draw pile
            Cards.RemoveAll(x => cardsDrew.Contains(x));

            return cardsDrew;
        }


        private void InitializeCards(List<CardValue> bannedCards)
        {
            Cards = new List<ICard>();

            AddNormalGameNormalCards();
            AddNormalGameNormalCards();
            AddNormalGameNormalCards();

            AddNormalGameWildCards();
            AddNormalGameWildCards();
            AddNormalGameWildCards();


            AddSpecialWildCards();
            AddSpecialWildCards();

            FilterBannedCards(bannedCards);

        }

        private void FilterBannedCards(List<CardValue> bannedCards)
        {
            Cards.RemoveAll(x => bannedCards.Contains(x.Value));
        }

        private void AddSpecialWildCards()
        {
            foreach (CardColor color in Enum.GetValues(typeof(CardColor)))
            {
                if (color != CardColor.Wild)
                {
                    Cards.Add(new StealTurn(color));
                    Cards.Add(new StealTurn(color));
                }
            }
            for (int i = 1; i <= 4; i++)
            {
                Cards.Add(new BlackHole());
                Cards.Add(new DiscardWildCards());
                Cards.Add(new SwapHands());
                Cards.Add(new DoubleEdge());
                Cards.Add(new DiscardColor());
                Cards.Add(new HandOfGod());
                Cards.Add(new Judgement());
                Cards.Add(new UnitedWeFall());
                Cards.Add(new ParadigmShift());
                Cards.Add(new InspectHand());
                Cards.Add(new GraveDigger());
                Cards.Add(new RussianRoulette());
                Cards.Add(new Roulette());
                Cards.Add(new Duel());
                Cards.Add(new FairPlay());
                Cards.Add(new TheLastStand());
                Cards.Add(new Charity());
                Cards.Add(new TricksOfTheTrade());
                Cards.Add(new Blackjack());
                Cards.Add(new DiscardNumber());
                Cards.Add(new FortuneTeller());
                //cads added 2 times
                Cards.Add(new MagneticPolarity());
                Cards.Add(new MagneticPolarity());
                Cards.Add(new KeepMyHand());
                Cards.Add(new KeepMyHand());
                Cards.Add(new Deflect());
                Cards.Add(new Deflect());
            }

        }

        private void AddNormalGameWildCards()
        {
            for (int i = 1; i <= 4; i++)
            {
                Cards.Add(new ChangeColor());
                Cards.Add(new DrawFour());
            }

        }

        private void AddNormalGameNormalCards()
        {
            foreach (CardColor color in Enum.GetValues(typeof(CardColor)))
            {
                //For every color we have defined
                if (color != CardColor.Wild)
                {
                    //Wild Cards don't have a color
                    foreach (CardValue val in Enum.GetValues(typeof(CardValue)))
                    {
                        switch (val)
                        {
                            case CardValue.One:
                            case CardValue.Two:
                            case CardValue.Three:
                            case CardValue.Four:
                            case CardValue.Five:
                            case CardValue.Six:
                            case CardValue.Seven:
                            case CardValue.Eight:
                            case CardValue.Nine:
                                Cards.Add(new Number(color, val));
                                Cards.Add(new Number(color, val));
                                break;
                            case CardValue.Skip:
                            case CardValue.Reverse:
                            case CardValue.DrawTwo:
                                Cards.Add(new DrawTwo(color));
                                Cards.Add(new DrawTwo(color));
                                break;
                            case CardValue.Zero:
                                //Add one copy per color for 0
                                Cards.Add(new Number(color, val));
                                break;
                        }
                    }
                }
            }
        }

        public void Shuffle()
        {
            Random r = new Random();

            for (int n = Cards.Count - 1; n > 0; --n)
            {
                int k = r.Next(n + 1);
                ICard temp = Cards[n];
                Cards[n] = Cards[k];
                Cards[k] = temp;
            }
        }

    }
}