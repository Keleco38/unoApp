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
            AddNormalGameWildCards(gameSetup.LimitColorChangingCards);

            if (gameSetup.GameType == GameType.SpecialWildCards)
            {
                AddNormalGameNormalCards();
                AddNormalGameNormalCards();

                AddStealTurnCards();
                AddStealTurnCards();
                AddStealTurnCards();

                AddSpecialWildCards(gameSetup.LimitColorChangingCards);
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

        private void AddSpecialWildCards(bool limitColorChangingCards)
        {
            for (int i = 1; i <= 4; i++)
            {
                Cards.Add(new BlackHole(limitColorChangingCards));
                Cards.Add(new DiscardWildCards(limitColorChangingCards));
                Cards.Add(new SwapHands(limitColorChangingCards));
                Cards.Add(new DoubleEdge(limitColorChangingCards));
                Cards.Add(new DiscardColor(limitColorChangingCards));
                Cards.Add(new HandOfGod(limitColorChangingCards));
                Cards.Add(new Judgement(limitColorChangingCards));
                Cards.Add(new UnitedWeFall(limitColorChangingCards));
                Cards.Add(new ParadigmShift(limitColorChangingCards));
                Cards.Add(new InspectHand(limitColorChangingCards));
                Cards.Add(new GraveDigger(limitColorChangingCards));
                Cards.Add(new RussianRoulette(limitColorChangingCards));
                Cards.Add(new Roulette(limitColorChangingCards));
                Cards.Add(new Duel(limitColorChangingCards));
                Cards.Add(new FairPlay(limitColorChangingCards));
                Cards.Add(new TheLastStand(limitColorChangingCards));
                Cards.Add(new Charity(limitColorChangingCards));
                Cards.Add(new TricksOfTheTrade(limitColorChangingCards));
                Cards.Add(new Blackjack(limitColorChangingCards));
                Cards.Add(new DiscardNumber(limitColorChangingCards));
                Cards.Add(new MagneticPolarity(limitColorChangingCards));
                Cards.Add(new FortuneTeller(limitColorChangingCards));
                Cards.Add(new DoubleDraw(limitColorChangingCards));
                Cards.Add(new Poison(limitColorChangingCards));
                Cards.Add(new RandomColor(limitColorChangingCards));
                Cards.Add(new PromiseKeeper(limitColorChangingCards));
                Cards.Add(new Gambling(limitColorChangingCards));
                Cards.Add(new CopyCat(limitColorChangingCards));
                Cards.Add(new RobinHood(limitColorChangingCards));
                Cards.Add(new Handcuff(limitColorChangingCards));
                //cads added 2 times
                Cards.Add(new KeepMyHand(limitColorChangingCards));
                Cards.Add(new KeepMyHand(limitColorChangingCards));
                Cards.Add(new Deflect(limitColorChangingCards));
                Cards.Add(new Deflect(limitColorChangingCards));
            }

        }

        private void AddNormalGameWildCards(bool limitColorChangingCards)
        {
            for (int i = 1; i <= 4; i++)
            {
                Cards.Add(new ChangeColor(limitColorChangingCards));
                Cards.Add(new DrawFour(limitColorChangingCards));
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
    }
}