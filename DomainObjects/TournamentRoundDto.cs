using System.Collections.Generic;

namespace DomainObjects
{
    public class TournamentRoundDto
    {
        public int RoundNumber { get; set; }
        public List<TournamentRoundGameDto> TournamentRoundGames { get; set; }
    }
}