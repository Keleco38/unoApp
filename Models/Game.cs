using System;
using System.Collections.Generic;
using System.Linq;
using Uno.Enums;

namespace Uno.Models
{
    public class Game
    {
        public Deck Deck { get; set; }
        public List<Player> Players { get; set; }
        public List<Card> DiscardPile { get; set; }
        public Direction Direction { get; set; }
        public Card LastCardPlayed { get; set; }
        public Player PlayerToPlay { get; set; }

        public Game(GameSetup gameSetup)
        {
            Deck = new Deck();
            InitializePlayers(gameSetup);
            PlayerToPlay = Players.First();
            LastCardPlayed = null;
        }



        public List<Card> Draw(int count)
        {
            var drawnDeck = Deck.Cards.Take(count).ToList();

            //Remove the drawn Deck from the draw pile
            Deck.Cards.RemoveAll(x => drawnDeck.Contains(x));

            return drawnDeck;
        }

        // -------------------------------------private------------


        private void InitializePlayers(GameSetup gameSetup)
        {
            Players = new List<Player>();
            foreach (var user in gameSetup.Users)
            {
                Players.Add(new Player(user, Draw(7)));
            }
        }
    }
}