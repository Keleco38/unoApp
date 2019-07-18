namespace Uno.Models.Dtos
{
    public class PlayerDto
    {
        public UserDto User { get; set; }
        public bool LeftGame { get; set; }
        public int NumberOfCards { get; set; }
        public int RoundsWonCount { get; set; }
    }
}