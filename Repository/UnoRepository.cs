using System;
using System.Collections.Generic;
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
            var user = _users.Find(x => x.ConnectionId == connectionId);
            return user;
        }

        public User GetUserByName(string name)
        {
            var user = _users.Find(x => x.Name == name);
            return user;
        }

        public Game GetGameByGameId(string gameId)
        {
            var game = _games.Find(x => x.Id == gameId);
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
