using System;
using System.Collections.Generic;
using Uno.Enums;
using Uno.Models;
using unoApp.Models.Helpers;

namespace unoApp.Models.Abstraction
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