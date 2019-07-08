using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Uno.Enums;
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
            var user = _users.Find(x => x.ConnectionId == Context.ConnectionId);

            await CleanupUserFromGames();
            await CleanupUserFromGameSetups();
            await CleanupUserFromUsers();

            await SendMessageToAllChat("Server", $"{user.Name} has left the server.", TypeOfMessage.Server);

            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessageToAllChat(string username, string message, TypeOfMessage typeOfMessage = TypeOfMessage.Chat)
        {
            ChatMessage msg = null;
            var user = _users.Find(x => x.ConnectionId == Context.ConnectionId);
            Regex regex = new Regex(@"^/(slap|buzz|alert) ([A-Za-z0-9\s]*)$");
            Match match = regex.Match(message);

            if (match.Success)
            {
                var targetedUsername = match.Groups[2].Value;
                var targetedUser = _users.FirstOrDefault(x => x.Name == targetedUsername);
                if (targetedUser != null)
                {
                    await Clients.Client(targetedUser.ConnectionId).SendAsync("BuzzPlayer");
                    await SendMessageToAllChat("Server", $"User {user.Name} has buzzed player {targetedUser.Name} ", TypeOfMessage.Server);
                }
                else
                {
                    msg = new ChatMessage("Server", $"Player {targetedUser.Name} not found", TypeOfMessage.Server);
                    await Clients.Caller.SendAsync("SendMessageToAllChat", msg);
                }
                return;
            }

            msg = new ChatMessage(username, message, typeOfMessage);
            await Clients.All.SendAsync("SendMessageToAllChat", msg);
        }


        public async Task SendMessageToGameChat(Guid gameId, string username, string message, TypeOfMessage typeOfMessage = TypeOfMessage.Chat)
        {
            ChatMessage msg = null;
            var game = _games.FirstOrDefault(x => x.Id == gameId);
            var user = _users.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            Regex regex = new Regex(@"^/(slap|buzz|alert) ([A-Za-z0-9\s]*)$");
            Match match = regex.Match(message);

            if (match.Success)
            {
                var targetedUsername = match.Groups[2].Value;
                var targetedUser = _users.FirstOrDefault(x => x.Name == targetedUsername);

                if (targetedUser != null)
                {
                    await Clients.Client(targetedUser.ConnectionId).SendAsync("BuzzPlayer");
                    await SendMessageToGameChat(gameId, "Server", $"User {user.Name} has buzzed player {targetedUser.Name} ", TypeOfMessage.Server);
                }
                else
                {
                    msg = new ChatMessage("Server", $"Player {targetedUser.Name} found", TypeOfMessage.Server);
                    await Clients.Caller.SendAsync("SendMessageToGameChat", msg);
                }
                return;
            }

            msg = new ChatMessage(username, message, typeOfMessage);

            var usersToNotify = GetPlayersFromGame(game);
            usersToNotify.AddRange(GetSpectatorsFromGame(game));

            await Clients.Clients(usersToNotify).SendAsync("SendMessageToGameChat", msg);
        }

        public async Task CreateNewGameSetup()
        {
            var user = _users.Find(x => x.ConnectionId == Context.ConnectionId);
            var gameSetup = new GameSetup(user);
            _gameSetups.Add(gameSetup);
            await UpdateLobby();

        }
        public async Task JoinGameSetup(Guid gameSetupId)
        {
            var user = _users.Find(x => x.ConnectionId == Context.ConnectionId);
            var gameSetup = _gameSetups.Find(x => x.Id == gameSetupId);
            gameSetup.Users.Add(user);
            await UpdateLobby();
        }

        public async Task CleanupUserFromGameSetups()
        {
            var user = _users.Find(x => x.ConnectionId == Context.ConnectionId);
            var gameSetup = _gameSetups.FirstOrDefault(x => x.Users.FirstOrDefault(y => y.Name == user.Name) != null);
            if (gameSetup != null)
            {
                gameSetup.Users.Remove(user);
                if (!gameSetup.Users.Any())
                {
                    _gameSetups.Remove(gameSetup);
                }
                await UpdateLobby();
            }
        }

        public async Task CreateNewGame(Guid gameSetupId)
        {
            var gameSetup = _gameSetups.Find(x => x.Id == gameSetupId);
            var game = new Game(gameSetup);
            _games.Add(game);
            _gameSetups.Remove(gameSetup);
            await UpdateLobby();
        }

        public async Task CleanupUserFromGames()
        {
            var user = _users.Find(x => x.ConnectionId == Context.ConnectionId);
            var game = _games.FirstOrDefault(x => x.Players.FirstOrDefault(y => y.User.Name == user.Name) != null);
            if (game != null)
            {
                var player = game.Players.Find(x => x.User == user);
                player.LeftGame = true;
                if (!game.Players.Any(x => x.LeftGame == false))
                {
                    _games.Remove(game);
                }
                await UpdateLobby();
            }
        }


        public async Task AddUser(string name)
        {
            User user;
            lock (_users)
            {
                name = Regex.Replace(name, @"\s+", "").ToLower();
                if (name.Length > 10)
                    name = name.Substring(0, 10);
                var nameExists = _users.Any(x => x.Name == name);
                if (!nameExists)
                {
                    user = new User(Context.ConnectionId, name);
                    _users.Add(user);
                }
                else
                {
                    Clients.Caller.SendAsync("RenamePlayer");
                }

            }
            await UpdateLobby();
        }

        public void DrawCard(Guid gameId)
        {
            var game = _games.Find(x => x.Id == gameId);
            lock (game)
            {
                if (game.PlayerToPlay.User.ConnectionId == Context.ConnectionId)
                {
                    game.DrawCard(game.PlayerToPlay, 1);
                }
            }
        }

        public async Task PlayCard(Guid gameId, Card card, CardColor cardColor)
        {
            var game = _games.Find(x => x.Id == gameId);
            lock (game)
            {
                if (game.PlayerToPlay.User.ConnectionId == Context.ConnectionId)
                {
                    var success = game.PlayCard(game.PlayerToPlay, card, cardColor);
                }
            }
            await Task.CompletedTask;
        }

        public async Task UpdateLobby()
        {
            await Clients.All.SendAsync("GetAllGames", _games);
            await Clients.All.SendAsync("GetAllGameSetups", _gameSetups);
            await Clients.All.SendAsync("GetAllUsers", _users);
        }



        //-------------------------- private

        private async Task CleanupUserFromUsers()
        {
            var user = _users.Find(x => x.ConnectionId == Context.ConnectionId);
            _users.Remove(user);
            await UpdateLobby();
        }

        private List<string> GetPlayersFromGame(Game game)
        {
            return game.Players.Where(x => !x.LeftGame).Select(y => y.User.ConnectionId).ToList();
        }

        private List<string> GetSpectatorsFromGame(Game game)
        {
            return game.Spectators.Select(y => y.ConnectionId).ToList();
        }



    }
}