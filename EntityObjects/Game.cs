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
        public List<ChatMessage> ChatMessages { get; set; }
        public List<string> GameLog { get; set; }
        public bool IsTournamentGame => !string.IsNullOrEmpty(TournamentId);
        public string TournamentId { get; set; }
        public Direction Direction { get; set; }
        public LastCardPlayed LastCardPlayed { get; set; }
        public Player PlayerToPlay { get; set; }
        public bool GameStarted { get; set; }
        public DateTime ReadyPhaseExpireUtc { get; set; }
        public List<string> ReadyPlayersLeft { get; set; }
        public bool RoundEnded { get; set; }
        public bool GameEnded { get; set; }
        public List<User> BannedUsers { get; set; }
        public Game(GameSetup gameSetup, string tournamentId = "")
        {
            Id = Guid.NewGuid().ToString();
            GameSetup = gameSetup;
            TournamentId = tournamentId;
            Players = new List<Player>();
            Spectators = new List<Spectator>();
            ChatMessages=new List<ChatMessage>();
            GameLog=new List<string>();
            ReadyPlayersLeft=new List<string>();
            ReadyPhaseExpireUtc=DateTime.Now;
            BannedUsers=new List<User>();
            DiscardedPile=new List<ICard>();
        }

    }
}