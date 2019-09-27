using System;
using System.Collections.Generic;

namespace EntityObjects
{
    public class Tournament
    {
        public Tournament(TournamentSetup tournamentSetup)
        {
            TournamentSetup = tournamentSetup;
            Id = Guid.NewGuid().ToString();
            Contestants = new List<Contestant>();
            TournamentRounds=new List<TournamentRound>();
            Spectators=new List<User>();
            ChatMessages = new List<ChatMessage>();
            ReadyPlayersLeft = new List<string>();
            ReadyPhaseExpireUtc = DateTime.Now;
        }
        public string Id { get; set; }
        public string Name { get; set; }
        public bool TournamentStarted { get; set; }
        public string TournamentWinner { get; set; }
        public bool TournamentEnded { get; set; }
        public TournamentSetup TournamentSetup { get; set; }
        public List<TournamentRound> TournamentRounds { get; set; }
        public DateTime ReadyPhaseExpireUtc { get; set; }
        public List<string> ReadyPlayersLeft { get; set; }
        public List<Contestant> Contestants { get; set; }
        public List<User> Spectators { get; set; }
        public List<ChatMessage> ChatMessages { get; set; }
    }
}