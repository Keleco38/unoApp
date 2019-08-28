using Common.Enums;

namespace EntityObjects.Cards.Abstraction
{
    public interface ICard
    {
        string Id { get; set; }
        CardColor Color { get; set; }
        CardValue Value { get; set; }
        string ImageUrl { get; set; }
    }
}