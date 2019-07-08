using Uno.Enums;

namespace Uno.Models
{
    public class ChatMessage
    {
        public ChatMessage(string name, string message, TypeOfMessage typeOfMessage)
        {
            Name = name;
            Message = message;
            TypeOfMessage=typeOfMessage;
        }
        public string Name { get; set; }
        public string Message { get; set; }
        public TypeOfMessage TypeOfMessage { get; set; }
    }
}