using Uno.Enums;

namespace Uno.Models
{
    public class Card
    {
        public Card(CardColor cardColor, CardValue cardValue)
        {
            Color=cardColor;
            Value=cardValue;
            ImageUrl = $"/images/cards/small/{(int)cardColor}/{(int)cardValue}.png";
        }
        public CardColor Color { get; set; }
        public CardValue Value { get; set; }
        public string ImageUrl { get; set; }

    }
}