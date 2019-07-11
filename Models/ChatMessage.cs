using Uno.Enums;

namespace Uno.Models
{
    public class ChatMessage
    {
        public ChatMessage(string name, string message, TypeOfMessage typeOfMessage)
        {
            Username = name;
            Text = message;
            TypeOfMessage=typeOfMessage;
        }
        public string Username { get; set; }
        public string Text { get; set; }
        public TypeOfMessage TypeOfMessage { get; set; }
    }
}