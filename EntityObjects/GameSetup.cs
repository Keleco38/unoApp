using System.Collections.Generic;
using Common.Enums;

namespace EntityObjects
{
    public class GameSetup
    {
        public GameSetup()
        {
            BannedCards=new List<CardValue>();
            RoundsToWin = 2;
        }
        public string Password { get; set; }
        public List<CardValue> BannedCards { get; set; }
        public int RoundsToWin { get; set; }
        public bool IsPasswordProtected => !string.IsNullOrEmpty(Password);
    }
}