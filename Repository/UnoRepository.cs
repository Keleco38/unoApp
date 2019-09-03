using System;
using System.Collections.Generic;
using System.Linq;
using EntityObjects;

namespace Repository
{
    public class UnoRepository : IUnoRepository
    {
        private readonly List<User> _users;
        private readonly List<Game> _games;

        public UnoRepository()
        {
            _users = new List<User>();
            _games = new List<Game>();
        }


        public User GetUserByConnectionId(string connectionId)
        {
            var user = _users.First(x => x.ConnectionId == connectionId);
            return user;
        }

        public bool UserExistsByName(string name)
        {
            return _users.Exists(x => x.Name == name);
        }

        public bool UserExistsByConnectionId(string connectionId)
        {
            return _users.Exists(x => x.ConnectionId == connectionId);
        }

        public User GetUserByName(string name)
        {
            var user = _users.First(x => x.Name == name);
            return user;
        }

        public Game GetGameByGameId(string gameId)
        {
            var game = _games.First(x => x.Id == gameId);
            return game;

        }

        public void RemoveGame(Game game)
        {
            _games.Remove(game);
        }

        public void RemoveUser(User user)
        {
            _users.Remove(user);
        }

        public void AddGame(Game game)
        {
            _games.Add(game);
        }

        public void AddUser(User user)
        {
            _users.Add(user);
        }

        public IReadOnlyList<User> GetAllUsers()
        {
            return _users.AsReadOnly();
        }
        public IReadOnlyList<Game> GetAllGames()
        {
            return _games.AsReadOnly();
        }

    }
}
