using System.Collections.Generic;

namespace unoApp.Models.Helpers
{
    public class MoveResult
    {
        public MoveResult(List<string> messagesToLog)
        {
            MessagesToLog = messagesToLog;
            MoveResultCallbackParams = new List<MoveResultCallbackParam>();
        }
        public MoveResult(List<string> messagesToLog, List<MoveResultCallbackParam> moveResultCallbackParams)
        {
            MessagesToLog = messagesToLog;
            MoveResultCallbackParams = moveResultCallbackParams;
        }

        public List<string> MessagesToLog { get; set; }
        public List<MoveResultCallbackParam> MoveResultCallbackParams { get; set; }

    }
}