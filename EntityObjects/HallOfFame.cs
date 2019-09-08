namespace EntityObjects
{
    public class HallOfFame
    {
        public HallOfFame(string name, int points,int gamesWon)
        {
            Name = name;
            Points = points;
            GamesWon = gamesWon;
        }

        public string Name { get; set; }
        public int Points { get; set; }
        public int GamesWon { get; set; }
    }
}