using System;
using Common.Enums;

namespace DomainObjects
{
    public class ChatMessageDto
    {
        public string Username { get; set; }
        public string Text { get; set; }
        public DateTime CreatedUtc { get; set; }
        public TypeOfMessage TypeOfMessage { get; set; }
    }
}