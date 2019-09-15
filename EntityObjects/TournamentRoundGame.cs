namespace EntityObjects
{
    public class TournamentRoundGame
    {
        public TournamentRoundGame(int gameNumber, Game game)
        {
            GameNumber = gameNumber;
            Game = game;
        }
        public int GameNumber { get; set; }
        public Game Game { get; set; }
    }
}