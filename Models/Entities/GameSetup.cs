using System;
using Uno.Enums;

namespace Uno.Models.Entities
{
    public class GameSetup
    {
        public GameSetup()
        {
            Password = string.Empty;
            GameMode = GameMode.SpecialCards;
            RoundsToWin = 2;
        }
        public string Password { get; set; }
        public GameMode GameMode { get; set; }
        public int RoundsToWin { get; set; }
        public bool IsPasswordProtected { get => Password.Length > 0; }
    }
}