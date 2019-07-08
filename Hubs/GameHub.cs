using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Uno.Enums;
using Uno.Models;
using Uno.Models.Dtos;

namespace Uno.Hubs
{
    public class GameHub : Hub
    {
        private readonly IMapper _mapper;
        private static List<User> _users { get; set; } = new List<User>();
        private static List<Game> _games { get; set; } = new List<Game>();

        public GameHub(IMapper mapper)
        {
            _mapper = mapper;
        }


        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(System.Exception exception)
        {
            var user = _users.Find(x => x.ConnectionId == Context.ConnectionId);

            await SendMessageToAllChat("Server", $"{user.Name} has left the server.", TypeOfMessage.Server);

            await CleanupUserFromGames();
            await CleanupUserFromUsersList();

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

        public async Task SendMessageToGameChat(string gameId, string username, string message, TypeOfMessage typeOfMessage = TypeOfMessage.Chat)
        {
            ChatMessage msg = null;
            var game = _games.FirstOrDefault(x => x.GameSetup.Id == gameId);
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

        public async Task SetGamePassword(string id, string password)
        {
            var game = _games.FirstOrDefault(x => x.GameSetup.Id == id);
            if (game == null)
                return;
            game.GameSetup.Password = password;
            await UpdateAllGames();
            await DisplayToastMessageToGame(id, "Password updated");
        }

        public async Task GetAllPlayers()
        {
            await Clients.All.SendAsync("GetAllPlayers", _users);
        }

        public async Task UpdateAllGames()
        {
            var games = _mapper.Map<List<GameDto>>(_games);
            await Clients.All.SendAsync("UpdateAllGames", games);
        }


        public async Task CreateGame(int playUntilPoints, int expectedNumberOfPlayers)
        {
            await CleanupUserFromGames();

            var user = _users.Find(x => x.ConnectionId == Context.ConnectionId);
            var gameSetup = new GameSetup();
            var game = new Game(gameSetup);
            game.Players.Add(new Player(user));
            _games.Add(game);
            await GameUpdated(game);
            await UpdateAllGames();
            await SendMessageToAllChat("Server", $"User {user.Name} has created new game", TypeOfMessage.Server);
        }

        public async Task ExitGame(string gameid)
        {
            var game = _games.SingleOrDefault(x => x.GameSetup.Id == gameid);

            if (game == null)
                return;

            var user = _users.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);

            var allSpectatorsFromTheGame = GetSpectatorsFromGame(game);
            var allPlayersFromGame = GetPlayersFromGame(game);

            if (allSpectatorsFromTheGame.Contains(Context.ConnectionId))
            {
                game.Spectators.Remove(game.Spectators.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId));
            }

            if (allPlayersFromGame.Contains(Context.ConnectionId))
            {
                var player = game.Players.FirstOrDefault(y => y.User.ConnectionId == Context.ConnectionId);
                if (game.GameStarted)
                {
                    player.LeftGame = true;
                    await DisplayToastMessageToGame(gameid, $"USER {player.User.Name} HAS LEFT THE GAME.");
                }
                else
                {
                    game.Players.Remove(player);
                }
            }


            if (!game.Players.Any(x => x.LeftGame == false) && !game.Spectators.Any())
                _games.Remove(game);

            await GameUpdated(game);
            await UpdateAllGames();
            await SendMessageToGameChat(gameid, "Server", $"{user.Name} has left the game.", TypeOfMessage.Server);
        }

        public async Task KickUserFromGame(string connectionId, string gameId)
        {
            var game = _games.FirstOrDefault(x => x.GameSetup.Id == gameId);
            if (game == null) return;

            var user = _users.FirstOrDefault(x => x.ConnectionId == connectionId);
            if (user == null) return;

            game.Players.Remove(game.Players.SingleOrDefault(y => y.User.ConnectionId == connectionId));

            await GameUpdated(game);
            await UpdateAllGames();
            await Clients.Client(connectionId).SendAsync("KickUSerFromGame");
        }


        public async Task StartGame(string gameId)
        {
            var game = _games.FirstOrDefault(x => x.GameSetup.Id == gameId);
            if (game == null) return;

            var user = _users.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            if (user == null) return;

            game.StartGame();

            await GameUpdated(game);
            await UpdateAllGames();
        }

        public async Task JoinGame(string gameId, string password)
        {
            await CleanupUserFromGamesExceptThisGame(gameId);
            var user = _users.Find(x => x.ConnectionId == Context.ConnectionId);
            var game = _games.Find(x => x.GameSetup.Id == gameId);

            var isAlreadySpectator = game.Spectators.Contains(user);

            if (!string.IsNullOrEmpty(game.GameSetup.Password) && !isAlreadySpectator)
                if (game.GameSetup.Password != password)
                    return;

            if (!game.GameStarted)
            {
                if (isAlreadySpectator)
                {
                    //join the gamt that hasn't started
                    game.Spectators.Remove(user);
                    game.Players.Add(new Player(user));
                }
                else
                {
                    //spectate game that hasn't started
                    game.Spectators.Add(user);
                    await SendMessageToGameChat(gameId, "Server", $"{user.Name} has joined the game room.", TypeOfMessage.Server);
                }
            }
            else
            {
                var playerLeftWithThisNickname = game.Players.FirstOrDefault(x => x.LeftGame && x.User.Name == user.Name);

                if (playerLeftWithThisNickname != null)
                {
                    playerLeftWithThisNickname.User = user;
                    playerLeftWithThisNickname.LeftGame = false;

                    if (game.PlayerToPlay.User.Name == user.Name)
                        game.PlayerToPlay = playerLeftWithThisNickname;
                    await DisplayToastMessageToGame(gameId, $"PLAYER {user.Name} HAS RECONNECTED TO THE GAME");
                    await SendMessageToGameChat(gameId, "Server", $"{user.Name} has joined the game room.", TypeOfMessage.Server);
                }
                else
                {
                    game.Spectators.Add(user);
                    await SendMessageToGameChat(gameId, "Server", $"{user.Name} has joined the game room.", TypeOfMessage.Server);
                }
            }


            await GameUpdated(game);
            await UpdateAllGames();
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
                    return;
                }

            }
            await GetAllPlayers();
            await Clients.Client(Context.ConnectionId).SendAsync("GetCurrentUser", user);
            await SendMessageToAllChat("Server", $"{user.Name} has connected to the server.", TypeOfMessage.Server);
            await base.OnConnectedAsync();
        }

        public void DrawCard(string gameId)
        {
            var game = _games.Find(x => x.GameSetup.Id == gameId);
            lock (game)
            {
                if (game.PlayerToPlay.User.ConnectionId == Context.ConnectionId)
                {
                    game.DrawCard(game.PlayerToPlay, 1);
                }
            }
        }

        public async Task PlayCard(string gameId, Card card, CardColor cardColor)
        {
            var game = _games.Find(x => x.GameSetup.Id == gameId);
            var user = _users.Find(x => x.ConnectionId == Context.ConnectionId); 
            lock (game)
            {
                if (game.GameEnded || !game.GameStarted || game.PlayerToPlay.User.Name != user.Name)
                    return;
                game.PlayCard(game.PlayerToPlay, card, cardColor);
            }
            await GameUpdated(game);
        }



        //-------------------------- private

        private async Task DisplayToastMessageToGame(string gameid, string message)
        {
            var game = _games.Find(x => x.GameSetup.Id == gameid);
            var usersToNotify = GetPlayersFromGame(game);
            usersToNotify.AddRange(GetSpectatorsFromGame(game));

            await Clients.Clients(usersToNotify).SendAsync("DisplayToastMessage", message);
        }

        private async Task DisplayToastMessageToUser(string connectionId, string message)
        {
            await Clients.Client(connectionId).SendAsync("DisplayToastMessage", message);
        }

        private async Task GameUpdated(Game game)
        {
            var allPlayersInTheGame = GetPlayersFromGame(game);
            var gameDto = _mapper.Map<GameDto>(game);

            var allSpectatorsInTheGame = GetSpectatorsFromGame(game);
            await Clients.Clients(allSpectatorsInTheGame).SendAsync("GameUpdate", gameDto);

            if (game.GameStarted)
            {
                foreach (var connectionId in allPlayersInTheGame)
                {
                    gameDto.MyCards = game.Players.FirstOrDefault(x => x.User.ConnectionId == connectionId).Cards;
                    await Clients.Client(connectionId).SendAsync("GameUpdate", gameDto);
                }
            }
            else
            {
                await Clients.Clients(allPlayersInTheGame).SendAsync("GameUpdate", gameDto);
            }

        }

        private List<string> GetPlayersFromGame(Game game)
        {
            return game.Players.Where(x => !x.LeftGame).Select(y => y.User.ConnectionId).ToList();
        }

        private List<string> GetSpectatorsFromGame(Game game)
        {
            return game.Spectators.Select(y => y.ConnectionId).ToList();
        }

        private async Task CleanupUserFromGames()
        {
            List<Game> games = _games.Where(x => GetPlayersFromGame(x).Where(y => y == Context.ConnectionId).Any()).ToList();

            games.AddRange(_games.Where(x => GetSpectatorsFromGame(x).Where(y => y == Context.ConnectionId).Any()).ToList());

            foreach (var game in games)
            {
                await ExitGame(game.GameSetup.Id);
            }
        }

        private async Task CleanupUserFromGamesExceptThisGame(string gameId)
        {
            List<Game> games = _games.Where(x => x.GameSetup.Id != gameId && GetPlayersFromGame(x).Where(y => y == Context.ConnectionId).Any()).ToList();

            games.AddRange(_games.Where(x => x.GameSetup.Id != gameId && GetSpectatorsFromGame(x).Where(y => y == Context.ConnectionId).Any()).ToList());

            foreach (var game in games)
            {
                await ExitGame(game.GameSetup.Id);
            }
        }

        private async Task CleanupUserFromUsersList()
        {
            var user = _users.Find(x => x.ConnectionId == Context.ConnectionId);
            _users.Remove(user);
            await GetAllPlayers();
        }

    }
}