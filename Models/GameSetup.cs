using System;
using System.Collections.Generic;
using Uno.Enums;

namespace Uno.Models
{
    public class GameSetup
    {
        public GameSetup()
        {
            Id = Guid.NewGuid().ToString();
            Password = string.Empty;
            GameMode = GameMode.SpecialCards;
            RoundsToWin = 3;
        }
        public string Id { get; set; }
        public string Password { get; set; }
        public GameMode GameMode { get; set; }
        public int RoundsToWin { get; set; }
        public bool IsPasswordProtected { get => Password.Length > 0; }
    }
}