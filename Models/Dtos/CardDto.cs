using Uno.Enums;

namespace Uno.Models.Dtos
{
    public class CardDto
    {
        public CardColor Color { get; set; }
        public CardValue Value { get; set; }
        public string ImageUrl { get; set; }
    }
}