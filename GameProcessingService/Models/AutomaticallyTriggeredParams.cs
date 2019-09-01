using System.Collections.Generic;
using EntityObjects;

namespace GameProcessingService.Models
{
    public class AutomaticallyTriggeredParams
    {
        public AutomaticallyTriggeredParams(MoveParams moveParams, string messageToLog, List<Player> playersAffected, int numberOfCardsToDraw)
        {
            MoveParams = moveParams;
            MessageToLog = messageToLog;
            PlayersAffected = playersAffected;
            NumberOfCardsToDraw = numberOfCardsToDraw;
        }
        public MoveParams MoveParams { get; set; }
        public string MessageToLog { get; set; }
        public List<Player> PlayersAffected { get; set; }
        public int NumberOfCardsToDraw { get; set; }
    }
}