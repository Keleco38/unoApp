using System.Collections.ObjectModel;
using EntityObjects;

namespace Repository
{
    public interface ITournamentRepository
    {
        void AddTournament(Tournament tournament);
        void RemoveTournament(Tournament tournament);
        Tournament GetTournament(string tournamentId);
        ReadOnlyCollection<Tournament> GetAllTournaments();
        bool TournamentExists(string tournamentId);
    }
}