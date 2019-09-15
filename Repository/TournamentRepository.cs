using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using EntityObjects;

namespace Repository
{
    public class TournamentRepository : ITournamentRepository
    {
        private readonly List<Tournament> _tournaments;

        public TournamentRepository()
        {
            _tournaments = new List<Tournament>();
        }

        public void AddTournament(Tournament tournament)
        {
            _tournaments.Add(tournament);
        }
        public void RemoveTournament(Tournament tournament)
        {
            _tournaments.Remove(tournament);
        }

        public Tournament GetTournament(string tournamentId)
        {
            return _tournaments.First(x => x.Id == tournamentId);
        }
        public ReadOnlyCollection<Tournament> GetAllTournaments()
        {
            return _tournaments.AsReadOnly();
        }
        public bool TournamentExists(string tournamentId)
        {
            return _tournaments.Exists(x => x.Id == tournamentId);
        }
    }
}