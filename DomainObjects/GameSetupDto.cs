using System.Collections.Generic;
using Common.Enums;

namespace DomainObjects
{
    public class GameSetupDto
    {
        public bool IsPasswordProtected { get; set; }
        public List<CardValue> BannedCards { get; set; }
        public int RoundsToWin { get; set; }
    }
}