using Uno.Enums;

namespace Uno.Models.Helpers
{
    public class ChatMessageIntentionResult
    {
        public ChatMessageIntention ChatMessageIntention { get; set; }
        public string TargetedUsername { get; set; }
        public string BuzzType { get; set; }
        public string BuzzTypeStringForChat { get; set; }
    }
}