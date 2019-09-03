using System.Collections.Generic;
using EntityObjects;

namespace Repository
{
    public interface IUnoRepository
    {
        User GetUserByConnectionId(string connectionId);
        User GetUserByName(string name);
        bool UserExistsByName(string name);
        bool UserExistsByConnectionId(string connectionId);
        Game GetGameByGameId(string gameId);
        void RemoveGame(Game game);
        void RemoveUser(User user);
        void AddGame(Game game);
        void AddUser(User user);
        IReadOnlyList<User> GetAllUsers();
        IReadOnlyList<Game> GetAllGames();
    }
}