using Uno.Enums;

namespace Uno.Models.Dtos
{
    public class ChatMessageDto
    {
        public string Username { get; set; }
        public string Text { get; set; }
        public TypeOfMessage TypeOfMessage { get; set; }
    }
}