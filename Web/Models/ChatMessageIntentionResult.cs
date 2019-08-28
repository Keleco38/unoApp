using System.Collections.Generic;
using Common.Enums;
using EntityObjects;

namespace Web.Models
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