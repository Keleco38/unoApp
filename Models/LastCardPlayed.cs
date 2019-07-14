using Uno.Enums;

namespace Uno.Models
{
    public class LastCardPlayed
    {
        public LastCardPlayed(CardColor color, CardValue value, string imageUrl, string playerPlayed)
        {
            Color = color;
            Value = value;
            ImageUrl = imageUrl;
            PlayerPlayed = playerPlayed;
        }
        public CardColor Color { get; set; }
        public CardValue Value { get; set; }
        public string ImageUrl { get; set; }
        public string PlayerPlayed { get; set; }
    }
}