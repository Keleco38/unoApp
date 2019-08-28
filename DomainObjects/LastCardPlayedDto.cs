using Common.Enums;

namespace DomainObjects
{
    public class LastCardPlayedDto
    {
        
        public CardColor Color { get; set; }
        public CardValue Value { get; set; }
        public bool WasWildCard { get; set; }
        public string ImageUrl { get; set; }
        public string PlayerPlayed { get; set; }
    }
}