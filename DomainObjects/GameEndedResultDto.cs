using System.Collections.Generic;

namespace DomainObjects
{
    public class GameEndedResultDto
    {
        public List<string> PlayersWon { get; set; }
        public int PointsWon { get; set; }
        public List<HallOfFameDto> HallOfFameStats { get; set; }

        public GameEndedResultDto(List<string> playersWon, int pointsWon, List<HallOfFameDto> hallOfFameStats)
        {
            PlayersWon = playersWon;
            PointsWon = pointsWon;
            HallOfFameStats = hallOfFameStats;
        }
    }
}