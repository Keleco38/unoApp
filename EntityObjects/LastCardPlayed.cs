using Common.Enums;
using EntityObjects.Cards.Abstraction;

namespace EntityObjects
{
    public class LastCardPlayed
    {
        public LastCardPlayed(CardColor color, CardValue value, string imageUrl, string playerPlayed, bool wasWildCard, ICard originalCardPlayer)
        {
            Color = color;
            Value = value;
            ImageUrl = imageUrl;
            PlayerPlayed = playerPlayed;
            WasWildCard=wasWildCard;
            OriginalCardPlayer = originalCardPlayer;
        }
        public CardColor Color { get; set; }
        public CardValue Value { get; set; }
        public bool WasWildCard { get; set; }
        public ICard OriginalCardPlayer { get; }
        public string ImageUrl { get; set; }
        public string PlayerPlayed { get; set; }
    }
}