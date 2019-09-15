using System.Collections.Generic;

namespace EntityObjects
{
    public class TournamentRound
    {
        public TournamentRound(int roundNumber)
        {
            RoundNumber = roundNumber;
            TournamentRoundGames=new List<TournamentRoundGame>();
        }
        public int RoundNumber { get; set; }
        public List<TournamentRoundGame> TournamentRoundGames { get; set; }
    }
}