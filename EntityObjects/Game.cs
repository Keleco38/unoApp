using System;
using System.Collections.Generic;
using System.Linq;
using Common.Enums;
using EntityObjects.Cards.Abstraction;

namespace EntityObjects
{
    public class Game
    {
        public string Id { get; set; }
        public Deck Deck { get; set; }
        public List<Player> Players { get; set; }
        public List<Spectator> Spectators { get; set; }
        public List<ICard> DiscardedPile { get; set; }
        public GameSetup GameSetup { get; set; }
        public Direction Direction { get; set; }
        public LastCardPlayed LastCardPlayed { get; set; }
        public Player PlayerToPlay { get; set; }
        public bool GameStarted { get; set; }
        public bool GameEnded { get; set; }

        public Game(GameSetup gameSetup)
        {
            Id = Guid.NewGuid().ToString();
            GameSetup = gameSetup;
            Players = new List<Player>();
            Spectators = new List<Spectator>();
        }
    }
}