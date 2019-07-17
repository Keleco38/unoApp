using Uno.Enums;
using Uno.Models;

namespace unoApp.Models.Helpers
{
    public class ChatMessageIntentionResult
    {
        public ChatMessageIntention ChatMessageIntention { get; set; }
        public string TargetedUsername { get; set; }
    }
}