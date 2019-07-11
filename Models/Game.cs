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
        public Card LastCardPlayed { get; set; }
        public Player PlayerToPlay { get; set; }
        public bool GameStarted { get; set; }
        public bool GameEnded { get; set; }

        public Game(GameSetup gameSetup)
        {
            GameSetup = gameSetup;
            Players = new List<Player>();
            Spectators=new List<Spectator>();
        }

        public bool PlayCard(Player player, Card card, CardColor cardColor)
        {
            if (PlayerToPlay != player)
                return false;

            if (card.Color != CardColor.Wild && card.Color != LastCardPlayed.Color && card.Value != LastCardPlayed.Value)
                return false;

            PlayerToPlay.Cards.Remove(card);
            DiscardedPile.Add(card);


            var gameEnded = DetectIfGameEnded();
            if (gameEnded)
            {
                GameEnded = true;
                return true;
            }

            if (card.Color == CardColor.Wild)
            {
                LastCardPlayed = new Card(cardColor, CardValue.ChangeColor);

                if (card.Value == CardValue.DrawFour)
                {
                    DrawCard(GetNextPlayerToPlay(), 4);
                }
            }
            else
            {
                LastCardPlayed = new Card(card.Color, card.Value);

                if (card.Value == CardValue.DrawTwo)
                {
                    DrawCard(GetNextPlayerToPlay(), 2);
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

            PlayerToPlay = GetNextPlayerToPlay();
            return true;
        }



        public void StartGame()
        {
            Deck = new Deck();
            DiscardedPile = Deck.Draw(1);
            LastCardPlayed = DiscardedPile.First();
            Direction = Direction.Right;
            PlayerToPlay = Players.First();
            Players.ForEach(x => x.Cards = Deck.Draw(7));
            GameStarted = true;
        }


        public void DrawCard(Player player, int count)
        {
            var handCount = player.Cards.Count;
            if (handCount + count > Constants.MAX_NUMBER_OF_CARDS)
            {
                count = Constants.MAX_NUMBER_OF_CARDS - handCount;
            }

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

            if (count == 1)
            {
                // if count is 1, then it's not a result of a wildcard
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