using System.Collections.Generic;

namespace unoApp.Models.Helpers
{
    public class TurnResult
    {
        public TurnResult()
        {
            Success = false;
            MessagesToLog = new List<string>();
        }
        public bool Success { get; set; }
        public List<string> MessagesToLog { get; set; }
    }
}