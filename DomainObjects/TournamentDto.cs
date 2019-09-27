using System;
using System.Collections.Generic;

namespace DomainObjects
{
    public class TournamentDto
    {
        public string Id { get; set; }
        public bool TournamentStarted { get; set; }
        public bool TournamentEnded { get; set; }
        public string TournamentWinner { get; set; }
        public TournamentSetupDto TournamentSetup { get; set; }
        public List<TournamentRoundDto> TournamentRounds { get; set; }
        public DateTime ReadyPhaseExpireUtc { get; set; }
        public List<string> ReadyPlayersLeft { get; set; }
        public List<ContestantDto> Contestants { get; set; }
        public List<UserDto> Spectators { get; set; }
    }
}