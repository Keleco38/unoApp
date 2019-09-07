using System.Collections.Generic;

namespace GameProcessingService.Models
{
    public class MoveResult
    {
        public MoveResult(string messageToLog, List<MoveResultCallbackParam> moveResultCallbackParams = null)
        {
            TurnMessageResult = messageToLog;
            MoveResultCallbackParams = moveResultCallbackParams ?? new List<MoveResultCallbackParam>();
        }

        public string TurnMessageResult { get; set; }
        public string RoundEndedMessageResult { get; set; }
        public string GameEndedMessageResult { get; set; }
        public List<MoveResultCallbackParam> MoveResultCallbackParams { get; set; }

    }
}