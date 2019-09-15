using EntityObjects;

namespace GameProcessingService.CoreManagers
{
    public interface ITournamentManager
    {
        void StartTournament(Tournament tournament);
        void UpdateTournament(Tournament tournament, Game gameEnded);
    }
}