using Uno.Enums;
using Uno.Models.Helpers;

namespace Uno.Models.Entities.Cards.Abstraction
{
    public interface ICard
    {
        string Id { get; set; }
        CardColor Color { get; set; }
        CardValue Value { get; set; }
        string ImageUrl { get; set; }
        MoveResult ProcessCardEffect(Game game, MoveParams moveParams);
    }
}