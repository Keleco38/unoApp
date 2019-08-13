using System;
using System.Collections.Generic;
using Uno.Enums;

namespace Uno.Models.Entities
{
    public class GameSetup
    {
        public GameSetup()
        {
            Password = string.Empty;
            BannedCards=new List<CardValue>();
            RoundsToWin = 2;
        }
        public string Password { get; set; }
        public List<CardValue> BannedCards { get; set; }
        public int RoundsToWin { get; set; }
        public bool IsPasswordProtected { get => Password.Length > 0; }
    }
}