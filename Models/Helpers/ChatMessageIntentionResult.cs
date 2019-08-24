using System.Collections.Generic;
using Uno.Enums;
using Uno.Models.Entities;

namespace Uno.Models.Helpers
{
    public class ChatMessageIntentionResult
    {
        public ChatMessageIntention ChatMessageIntention { get; set; }
        public string TargetedUsername { get; set; }
        public string BuzzType { get; set; }
        public string BuzzTypeStringForChat { get; set; }
        public List<User> MentionedUsers { get; set; }
    }
}