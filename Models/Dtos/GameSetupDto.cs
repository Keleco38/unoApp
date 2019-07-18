using Uno.Enums;

namespace Uno.Models.Dtos
{
    public class GameSetupDto
    {
        public string Id { get; set; }
        public bool IsPasswordProtected { get; set; }
        public GameMode GameMode { get; set; }
        public int RoundsToWin { get; set; }
    }
}