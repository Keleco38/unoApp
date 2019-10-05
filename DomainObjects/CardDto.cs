using Common.Enums;

namespace DomainObjects
{
    public class CardDto
    {
        public string Id { get; set; }
        public CardColor Color { get; set; }
        public CardValue Value { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        public bool RequirePickColor { get; set; }
        public bool RequireTargetPlayer { get; set; }
    }
}