using EntityObjects;

namespace PreMoveProcessingService.CoreManagers
{
    public interface ITournamentManager
    {
        void StartTournament(Tournament tournament);
        void UpdateTournament(Tournament tournament, Game gameEnded);
    }
}