using System.Collections.Generic;
using EntityObjects;

namespace Repository
{
    public interface IGameRepository
    {
        IReadOnlyList<Game> GetAllGames();
        void AddGame(Game game);
        Game GetGameByGameId(string gameId);
        void RemoveGame(Game game);

    }
}