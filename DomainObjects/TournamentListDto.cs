namespace DomainObjects
{
    public class TournamentListDto
    {
        public string Id { get; set; }
        public int NumberOfPlayers { get; set; }
        public int RequiredNumberOfPlayers { get; set; }
        public bool IsPasswordProtected { get; set; }
        public string Name { get; set; }
        public bool TournamentStarted { get; set; }
    }
}