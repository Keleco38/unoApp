using System;
using Common.Enums;

namespace EntityObjects
{
    public class ChatMessage
    {
        public ChatMessage(string name, string message, TypeOfMessage typeOfMessage)
        {
            Username = name;
            Text = message;
            TypeOfMessage=typeOfMessage;
            CreatedUtc=DateTime.Now;
        }
        public string Username { get; set; }
        public DateTime CreatedUtc { get; set; }
        public string Text { get; set; }
        public TypeOfMessage TypeOfMessage { get; set; }
    }
}