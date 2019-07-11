using System;
using System.Collections.Generic;
using Uno.Enums;

namespace Uno.Models
{
    public class GameSetup
    {
        public GameSetup(GameMode gameMode)
        {
            Id = Guid.NewGuid().ToString();
            Password = string.Empty;
            GameMode = gameMode;
        }
        public string Id { get; set; }
        public string Password { get; set; }
        public GameMode GameMode { get; set; }
        public bool IsPasswordProtected { get => Password.Length > 0; }
    }
}