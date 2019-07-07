using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Uno.Models;

namespace Uno.Hubs
{
    public class GameHub : Hub
    {
        private static List<User> _users { get; set; } = new List<User>();
        private static List<GameSetup> _gameSetups { get; set; } = new List<GameSetup>();
        private static List<Game> _games { get; set; } = new List<Game>();

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(System.Exception exception)
        {
            await CleanupUserFromGames();
            await CleanupUserGameSetups();
            await CleanupUserFromUsers();

            await base.OnDisconnectedAsync(exception);
        }



        public void CreateNewGameSetup()
        {
            var user = _users.Find(x => x.ConnectionId == Context.ConnectionId);
            var gameSetup = new GameSetup(user);
            _gameSetups.Add(gameSetup);
        }

        public void JoinGameSetup(Guid gameSetupId)
        {
            var user = _users.Find(x => x.ConnectionId == Context.ConnectionId);
            var gameSetup = _gameSetups.Find(x => x.Id == gameSetupId);
            gameSetup.Users.Add(user);
        }

        public void CreateNewGame(Guid gameSetupId)
        {
            var gameSetup = _gameSetups.Find(x => x.Id == gameSetupId);
            var game = new Game(gameSetup);
            _games.Add(game);
            _gameSetups.Remove(gameSetup);
        }

        public void AddUser(string name)
        {

            User user;
            lock (_users)
            {
                name = Regex.Replace(name, @"\s+", "").ToLower();

                if (name.Length > 10)
                    name = name.Substring(0, 10);

                var nameExists = _users.Any(x => x.Name == name);
                if (nameExists)
                {
                    Random rnd = new Random();
                    name = name + rnd.Next(1, 100);
                }


                user = new User(Context.ConnectionId, name);

                _users.Add(user);
            }
        }



        //-------------------------- private
        private async Task CleanupUserFromGames()
        {
            var user = _users.Find(x => x.ConnectionId == Context.ConnectionId);
            var game = _games.FirstOrDefault(x => x.Players.FirstOrDefault(y => y.User == user) != null);
            if (game != null)
            {
                var player = game.Players.Find(x => x.User == user);
                player.LeftGame = true;
                if (!game.Players.Any(x=>x.LeftGame==false))
                {
                    _games.Remove(game);
                }
            }
            await Task.CompletedTask;
        }

        private async Task CleanupUserFromUsers()
        {
            var user = _users.Find(x => x.ConnectionId == Context.ConnectionId);
            _users.Remove(user);
            await Task.CompletedTask;
        }

        private async Task CleanupUserGameSetups()
        {
            var user = _users.Find(x => x.ConnectionId == Context.ConnectionId);
            var gameSetup = _gameSetups.FirstOrDefault(x => x.Users.Contains(user));
            if (gameSetup != null)
            {
                gameSetup.Users.Remove(user);
                if (!gameSetup.Users.Any())
                {
                    _gameSetups.Remove(gameSetup);
                }
            }
            await Task.CompletedTask;
        }

    }
}