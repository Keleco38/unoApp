using System.Collections.Generic;
using EntityObjects;

namespace GameProcessingService.Models
{
    public class AutomaticallyTriggeredResult
    {
        public string MessageToLog { get; set; }
        public int NumberOfCardsToDraw { get; set; }
        public List<Player> PlayersWithoutKeepMyHand { get; set; }
        public Player MagneticPolaritySelectedPlayer { get; set; }
    }
}