using System.Collections.Generic;
using EntityObjects;

namespace GameProcessingService.Models
{
    public class AutomaticallyTriggeredResult
    {
        public AutomaticallyTriggeredResult(string messageToLog, int numberOfCardsToDraw = 0, List<Player> playersWithoutKeepMyHand = null)
        {
            MessageToLog = messageToLog;
            NumberOfCardsToDraw = numberOfCardsToDraw;
            PlayersWithoutKeepMyHand = playersWithoutKeepMyHand;
        }

        public string MessageToLog { get; set; }
        public int NumberOfCardsToDraw { get; set; }
        public List<Player> PlayersWithoutKeepMyHand { get; set; }
    }
}