using System;
using System.Collections.Generic;
using System.Linq;
using Uno.Enums;

namespace Uno.Models
{
    public class Deck
    {
        public Deck(GameMode gameMode)
        {
            InitializeCards(gameMode);
            Shuffle();
        }

        public List<Card> Cards { get; set; }


        public List<Card> Draw(int count)
        {
            var cardsDrew = Cards.Take(count).ToList();

            //Remove the drawn Deck from the draw pile
            Cards.RemoveAll(x => cardsDrew.Contains(x));

            return cardsDrew;
        }


        private void InitializeCards(GameMode gameMode)
        {
            Cards = new List<Card>();

            AddNormalGameNormalCards();
            AddNormalGameWildCards();

            if (gameMode == GameMode.SpecialCards || gameMode == GameMode.SpecialCardsAndAvalonCards)
            {
               // AddNormalGameNormalCards();
                AddSpecialWildCards();
            }
            if (gameMode == GameMode.SpecialCardsAndAvalonCards)
            {
                //todo
            }
        }

        private void AddSpecialWildCards()
        {
            foreach (CardColor color in Enum.GetValues(typeof(CardColor)))
            {
                if (color != CardColor.Wild)
                {
                    Cards.Add(new Card(color, CardValue.StealTurn));
                    Cards.Add(new Card(color, CardValue.StealTurn));
                }
            }
            for (int i = 1; i <= 4; i++)
            {
                Cards.Add(new Card(CardColor.Wild, CardValue.BlackHole));
                Cards.Add(new Card(CardColor.Wild, CardValue.DiscardAllWildCards));
                Cards.Add(new Card(CardColor.Wild, CardValue.SwapHands));
                Cards.Add(new Card(CardColor.Wild, CardValue.HandOfGod));
            }
        }

        private void AddNormalGameWildCards()
        {
            for (int i = 1; i <= 4; i++)
            {
                Cards.Add(new Card(CardColor.Wild, CardValue.ChangeColor));
                Cards.Add(new Card(CardColor.Wild, CardValue.DrawFour));
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
                            case CardValue.Skip:
                            case CardValue.Reverse:
                            case CardValue.DrawTwo:
                                Cards.Add(new Card(color, val));
                                Cards.Add(new Card(color, val));
                                break;
                            case CardValue.Zero:
                                //Add one copy per color for 0
                                Cards.Add(new Card(color, val));
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
                Card temp = Cards[n];
                Cards[n] = Cards[k];
                Cards[k] = temp;
            }
        }



    }
}