using System.Collections.Generic;
using System.Linq;
using EntityObjects;

namespace Repository
{
    public class GameRepository:IGameRepository
    {
        private readonly List<Game> _games;

        public GameRepository()
        {
            _games = new List<Game>();
        }

        public Game GetGameByGameId(string gameId)
        {
            var game = _games.First(x => x.Id == gameId);
            return game;

        }
        public void AddGame(Game game)
        {
            _games.Add(game);
        }

        public IReadOnlyList<Game> GetAllGames()
        {
            return _games.AsReadOnly();
        }

        public void RemoveGame(Game game)
        {
            _games.Remove(game);
        }
    }
}