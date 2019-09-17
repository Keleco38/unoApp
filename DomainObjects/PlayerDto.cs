namespace DomainObjects
{
    public class PlayerDto
    {
        public string Id { get; set; }
        public UserDto User { get; set; }
        public bool LeftGame { get; set; }
        public int PositionInGame { get; set; }
        public string TeamNumber { get; set; }
        public int NumberOfCards { get; set; }
        public int RoundsWonCount { get; set; }
    }
}