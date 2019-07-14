using System;
using System.Collections.Generic;
using System.Linq;
using Uno.Contants;
using Uno.Enums;

namespace Uno.Models
{
    public class Game
    {
        public Deck Deck { get; set; }
        public List<Player> Players { get; set; }
        public List<Spectator> Spectators { get; set; }
        public List<Card> DiscardedPile { get; set; }
        public GameSetup GameSetup { get; set; }
        public Direction Direction { get; set; }
        public LastCardPlayed LastCardPlayed { get; set; }
        public Player PlayerToPlay { get; set; }
        public bool GameStarted { get; set; }
        public bool GameEnded { get; set; }

        public Game(GameSetup gameSetup)
        {
            GameSetup = gameSetup;
            Players = new List<Player>();
            Spectators = new List<Spectator>();
            DiscardedPile = new List<Card>();
        }

        public bool PlayCard(Player player, Card cardPlayed, CardColor pickedCardColor)
        {
            var card = PlayerToPlay.Cards.Find(y => y.Color == cardPlayed.Color && y.Value == cardPlayed.Value);

            if (PlayerToPlay != player)
                return false;

            if (card.Color != CardColor.Wild && card.Color != LastCardPlayed.Color && card.Value != LastCardPlayed.Value)
                return false;


            PlayerToPlay.Cards.Remove(card);
            DiscardedPile.Add(card);


            LastCardPlayed = new LastCardPlayed(pickedCardColor, card.Value, card.ImageUrl, player.User.Name);

            if (card.Color == CardColor.Wild)
            {
                if (card.Value == CardValue.DrawFour)
                {
                    DrawCard(GetNextPlayerToPlay(), 4, false);
                }
                if (card.Value == CardValue.DiscardAllWildCards)
                {
                    Players.ForEach(x =>
                    {
                        var wildCards = x.Cards.Where(y => y.Color == CardColor.Wild).ToList();
                        DiscardedPile.AddRange(wildCards);
                        wildCards.ForEach(y => x.Cards.Remove(y));
                    });
                }
                if (card.Value == CardValue.BlackHole)
                {
                    Players.ForEach(x =>
                    {
                        var cardCount = x.Cards.Count;
                        DiscardedPile.AddRange(x.Cards.ToList());
                        x.Cards.Clear();
                        DrawCard(x, cardCount, false);
                    });
                }
            }
            else
            {
                if (card.Value == CardValue.DrawTwo)
                {
                    DrawCard(GetNextPlayerToPlay(), 2, false);
                }
                else if (card.Value == CardValue.Reverse)
                {
                    Direction = Direction == Direction.Right ? Direction.Left : Direction.Right;
                }
                else if (card.Value == CardValue.Skip)
                {
                    PlayerToPlay = GetNextPlayerToPlay();
                }
            }

            GameEnded = DetectIfGameEnded();
            PlayerToPlay = GetNextPlayerToPlay();
            return true;
        }



        public void StartGame()
        {
            Card lastCardDrew;
            Deck = new Deck(GameSetup.GameMode);
            do
            {
                lastCardDrew = Deck.Draw(1).First();
                DiscardedPile.Add(lastCardDrew);
            } while (lastCardDrew.Color == CardColor.Wild);
            LastCardPlayed = new LastCardPlayed(lastCardDrew.Color, lastCardDrew.Value, lastCardDrew.ImageUrl, string.Empty);
            Direction = Direction.Right;
            PlayerToPlay = Players.First();
            Players.ForEach(x => x.Cards = Deck.Draw(7));
            GameStarted = true;
        }


        public void DrawCard(Player player, int count, bool normalDraw)
        {
            var deckCount = Deck.Cards.Count;
            if (deckCount < count)
            {
                player.Cards.AddRange(Deck.Draw(deckCount));
                Deck.Cards = DiscardedPile.ToList();
                Deck.Shuffle();
                DiscardedPile.Clear();
                player.Cards.AddRange(Deck.Draw(count - deckCount));
            }
            else
            {
                player.Cards.AddRange(Deck.Draw(count));
            }

            if (normalDraw)
            {
                // if it's normalDraw then it's not a result of a wildcard
                PlayerToPlay = GetNextPlayerToPlay();
            }
        }


        // -------------------------------------private------------


        private Player GetNextPlayerToPlay()
        {
            var indexOfCurrentPlayer = Players.IndexOf(PlayerToPlay);
            if (Direction == Direction.Right)
            {
                if (indexOfCurrentPlayer == Players.Count - 1)
                {
                    return Players.First();
                }
                else
                {
                    return Players[indexOfCurrentPlayer + 1];
                }
            }
            if (Direction == Direction.Left)
            {
                if (indexOfCurrentPlayer == 0)
                {
                    return Players.Last();
                }
                else
                {
                    return Players[indexOfCurrentPlayer - 1];
                }
            }
            throw new Exception("Error, can't access that direction");

        }

        private bool DetectIfGameEnded()
        {
            return !PlayerToPlay.Cards.Any();
        }
    }
}