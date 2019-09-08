using System.Collections.Generic;
using EntityObjects;

namespace Repository
{
    public interface IHallOfFameRepository
    {
        void AddPoints(List<string> usernames, int points);
        List<HallOfFame> GetTopFiftyPlayers();
        List<HallOfFame> GetScoresForUsernames(List<string> usernames);
    }
}