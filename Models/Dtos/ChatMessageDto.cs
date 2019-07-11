using Uno.Enums;

namespace Uno.Models.Dtos
{
    public class ChatMessageDto
    {
        public string Name { get; set; }
        public string Message { get; set; }
        public TypeOfMessage TypeOfMessage { get; set; }
    }
}