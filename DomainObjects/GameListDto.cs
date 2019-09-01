namespace DomainObjects
{
    public class GameListDto
    {
        public string Id { get; set; }
        public int NumberOfPlayers { get; set; }
        public bool IsPasswordProtected { get; set; }
        public string Host { get; set; }
        public bool GameStarted { get; set; }
    }
}