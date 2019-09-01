using System;
using System.Collections.Generic;
using System.Linq;
using Common.Enums;
using EntityObjects.Cards.Abstraction;
using EntityObjects.Cards.Colored;
using EntityObjects.Cards.Wild;

namespace EntityObjects
{
    public class Deck
    {
        public Deck(GameSetup gameSetup)
        {
            InitializeCards(gameSetup);
            Shuffle();
        }

        public List<ICard> Cards { get; set; }

        public List<ICard> Draw(int count)
        {
            var cardsDrew = Cards.Take(count).ToList();
            Cards.RemoveAll(x => cardsDrew.Contains(x));
            return cardsDrew;
        }

        private void InitializeCards(GameSetup gameSetup)
        {
            Cards = new List<ICard>();

            AddNormalGameNormalCards();
            AddNormalGameWildCards();

            if (gameSetup.GameType == GameType.SpecialWildCards)
            {
                AddNormalGameNormalCards();
                AddNormalGameNormalCards();

                AddStealTurnCards();
                AddStealTurnCards();
                AddStealTurnCards();

                AddSpecialWildCards();
            }


            FilterBannedCards(gameSetup.BannedCards);
        }

        private void AddStealTurnCards()
        {
            foreach (CardColor color in Enum.GetValues(typeof(CardColor)))
            {
                if (color != CardColor.Wild)
                {
                    Cards.Add(new StealTurn(color));
                    Cards.Add(new StealTurn(color));
                }
            }
        }

        private void FilterBannedCards(List<CardValue> bannedCards)
        {
            Cards.RemoveAll(x => bannedCards.Contains(x.Value));
        }

        private void AddSpecialWildCards()
        {
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
                Cards.Add(new MagneticPolarity());
                Cards.Add(new FortuneTeller());
                Cards.Add(new DoubleDraw());
                Cards.Add(new Poison());
                Cards.Add(new RandomColor());
                Cards.Add(new PromiseKeeper());
                Cards.Add(new Gambling());
                //cads added 2 times
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
                if (color != CardColor.Wild)
                {
                    Cards.Add(new Zero(color));
                    Cards.Add(new One(color));
                    Cards.Add(new One(color));
                    Cards.Add(new Two(color));
                    Cards.Add(new Two(color));
                    Cards.Add(new Three(color));
                    Cards.Add(new Three(color));
                    Cards.Add(new Four(color));
                    Cards.Add(new Four(color));
                    Cards.Add(new Five(color));
                    Cards.Add(new Five(color));
                    Cards.Add(new Six(color));
                    Cards.Add(new Six(color));
                    Cards.Add(new Seven(color));
                    Cards.Add(new Seven(color));
                    Cards.Add(new Eight(color));
                    Cards.Add(new Eight(color));
                    Cards.Add(new Nine(color));
                    Cards.Add(new Nine(color));
                    Cards.Add(new Skip(color));
                    Cards.Add(new Skip(color));
                    Cards.Add(new Reverse(color));
                    Cards.Add(new Reverse(color));
                    Cards.Add(new DrawTwo(color));
                    Cards.Add(new DrawTwo(color));
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