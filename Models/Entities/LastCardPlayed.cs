using Uno.Enums;

namespace Uno.Models.Entities
{
    public class LastCardPlayed
    {
        public LastCardPlayed(CardColor color, CardValue value, string imageUrl, string playerPlayed, bool wasWildCard)
        {
            Color = color;
            Value = value;
            ImageUrl = imageUrl;
            PlayerPlayed = playerPlayed;
            WasWildCard=wasWildCard;
        }
        public CardColor Color { get; set; }
        public CardValue Value { get; set; }
        public bool WasWildCard { get; set; }
        public string ImageUrl { get; set; }
        public string PlayerPlayed { get; set; }
    }
}