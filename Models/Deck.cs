using System;
using System.Collections.Generic;
using Uno.Enums;

namespace Uno.Models
{
    public class Deck
    {
        public Deck()
        {
            InitializeCards();
            Shuffle();
        }

        public List<Card> Cards { get; set; }


        private void InitializeCards()
        {
            Cards = new List<Card>();

            //For every color we have defined
            foreach (CardColor color in Enum.GetValues(typeof(CardColor)))
            {
                if (color != CardColor.Wild) //Wild Cards don't have a color
                {
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
                                //Add two copies of each color card 1-9
                                Cards.Add(new Card()
                                {
                                    Color = color,
                                    Value = val
                                });
                                Cards.Add(new Card()
                                {
                                    Color = color,
                                    Value = val
                                });
                                break;
                            case CardValue.Skip:
                            case CardValue.Reverse:
                            case CardValue.DrawTwo:
                                //Add two copies per color of Skip, Reverse, and Draw Two
                                Cards.Add(new Card()
                                {
                                    Color = color,
                                    Value = val
                                });
                                Cards.Add(new Card()
                                {
                                    Color = color,
                                    Value = val
                                });
                                break;

                            case CardValue.Zero:
                                //Add one copy per color for 0
                                Cards.Add(new Card()
                                {
                                    Color = color,
                                    Value = val
                                });
                                break;
                        }
                    }
                }
                else //Handle wild Cards here
                {
                    //Add four regular wild Cards
                    for (int i = 1; i <= 4; i++)
                    {
                        Cards.Add(new Card()
                        {
                            Color = color,
                            Value = CardValue.Wild,
                        });
                    }
                    //Add four Draw Four Wild Cards
                    for (int i = 1; i <= 4; i++)
                    {
                        Cards.Add(new Card()
                        {
                            Color = color,
                            Value = CardValue.DrawFour,
                        });
                    }
                }
            }
        }

        private void Shuffle()
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