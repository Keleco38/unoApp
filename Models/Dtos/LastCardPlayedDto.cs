using Uno.Enums;

namespace Uno.Models.Dtos
{
    public class LastCardPlayedDto
    {
        
        public CardColor Color { get; set; }
        public CardValue Value { get; set; }
        public string ImageUrl { get; set; }
        public string PlayerPlayed { get; set; }
    }
}