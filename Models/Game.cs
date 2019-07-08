using System;
using System.Collections.Generic;
using System.Linq;
using Uno.Enums;

namespace Uno.Models
{
    public class Game
    {
        public Guid Id { get; set; }
        public Deck Deck { get; set; }
        public List<Player> Players { get; set; }
        public List<Card> DiscardedPile { get; set; }
        public Direction Direction { get; set; }
        public Card LastCardPlayed { get; set; }
        public Player PlayerToPlay { get; set; }
        public bool GameEnded { get; set; }

        public Game(GameSetup gameSetup)
        {
            Deck = new Deck();
            DiscardedPile = Deck.Draw(1);
            LastCardPlayed = DiscardedPile.First();
            InitializePlayers(gameSetup);
            Id = gameSetup.Id;
            Direction = Direction.Right;
            PlayerToPlay = Players.First();
            GameEnded=false;
        }

        public bool PlayCard(Player player, Card card, CardColor cardColor)
        {
            if (PlayerToPlay != player)
                return false;

            if (card.Color != CardColor.Wild && card.Color != LastCardPlayed.Color && card.Value != LastCardPlayed.Value)
                return false;
            
            PlayerToPlay.Hand.Remove(card);
            DiscardedPile.Add(card);


            var gameEnded=DetectIfGameEnded();
            if(gameEnded){
                GameEnded=true;
                return true;
            }

            if (card.Color == CardColor.Wild)
            {
                LastCardPlayed = new Card(cardColor,CardValue.ChangeColor);

                if (card.Value == CardValue.DrawFour)
                {
                    DrawCard(GetNextPlayerToPlay(), 4);
                }
            }
            else
            {
                LastCardPlayed = new Card(card.Color,card.Value);
          
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

        private bool DetectIfGameEnded()
        {
            return !PlayerToPlay.Hand.Any();
        }

        public void DrawCard(Player player, int count)
        {
            var deckCount = Deck.Cards.Count;
            if (deckCount < count)
            {
                player.Hand.AddRange(Deck.Draw(deckCount));
                Deck.Cards = DiscardedPile.ToList();
                Deck.Shuffle();
                DiscardedPile.Clear();
                player.Hand.AddRange(Deck.Draw(count - deckCount));
            }
            else
            {
                player.Hand.AddRange(Deck.Draw(count));
            }

            if (count == 1)
            {
                // if count is 1, then it's not a result of a wildcard
                PlayerToPlay = GetNextPlayerToPlay();
            }
        }




        // -------------------------------------private------------


        private void InitializePlayers(GameSetup gameSetup)
        {
            Players = new List<Player>();
            foreach (var user in gameSetup.Users)
            {
                Players.Add(new Player(user, Deck.Draw(7)));
            }
        }

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


    }
}