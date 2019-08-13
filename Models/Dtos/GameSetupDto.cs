using System.Collections.Generic;
using Uno.Enums;

namespace Uno.Models.Dtos
{
    public class GameSetupDto
    {
        public bool IsPasswordProtected { get; set; }
        public List<CardValue> BannedCards { get; set; }
        public int RoundsToWin { get; set; }
    }
}