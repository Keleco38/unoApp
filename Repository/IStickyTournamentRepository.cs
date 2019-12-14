using System.Collections.Generic;
using EntityObjects;

namespace Repository
{
    public interface IStickyTournamentRepository
    {
        void AddOrUpdateStickyTournament(string name, string url);
        void DeleteStickyTournament(string name);
        List<StickyTournament> GetAllStickyTournaments();
    }
}