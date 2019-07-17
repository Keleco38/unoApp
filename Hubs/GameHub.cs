using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Uno.Contants;
using Uno.Enums;
using Uno.Models;
using Uno.Models.Dtos;
using unoApp.Models.Helpers;

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
            await CleanupUserFromOnlineUsersList();

            await base.OnDisconnectedAsync(exception);
        }
        public async Task SendMessageToAllChat(string username, string message, TypeOfMessage typeOfMessage = TypeOfMessage.Chat)
        {
            var user = _users.Find(x => x.ConnectionId == Context.ConnectionId);

            var chatMessageIntentionResult = GetChatMessageIntention(message);

            if (chatMessageIntentionResult.ChatMessageIntention == ChatMessageIntention.Buzz)
            {
                var targetedUser = _users.FirstOrDefault(x => x.Name == chatMessageIntentionResult.TargetedUsername);
                if (targetedUser != null)
                {
                    var canBeBuzzedAfter = targetedUser.LastBuzzedUtc.AddSeconds(Constants.MINIMUM_TIME_SECONDS_BETWEEN_BUZZ);
                    if (DateTime.Now > canBeBuzzedAfter)
                    {
                        targetedUser.LastBuzzedUtc=DateTime.Now;
                        await Clients.Client(targetedUser.ConnectionId).SendAsync("BuzzPlayer");
                        var msgDto = _mapper.Map<ChatMessageDto>(new ChatMessage("Server", $"User {user.Name} has buzzed player {targetedUser.Name}", TypeOfMessage.Server));
                        await Clients.All.SendAsync("PostNewMessageInAllChat", msgDto);
                    }
                    else
                    {
                        var msgDto = _mapper.Map<ChatMessageDto>(new ChatMessage("Server", $"User {targetedUser.Name} was not buzzed! Wait 5 seconds.", TypeOfMessage.Server));
                        await Clients.Caller.SendAsync("PostNewMessageInAllChat", msgDto);
                    }

                }
                else
                {
                    var msgDto = _mapper.Map<ChatMessageDto>(new ChatMessage("Server", $"Player {targetedUser.Name} not found", TypeOfMessage.Server));
                    await Clients.Caller.SendAsync("PostNewMessageInAllChat", msgDto);
                }
            }
            else if (chatMessageIntentionResult.ChatMessageIntention == ChatMessageIntention.Normal)
            {
                var msgDto = _mapper.Map<ChatMessageDto>(new ChatMessage(username, message, typeOfMessage));
                await Clients.All.SendAsync("PostNewMessageInAllChat", msgDto);
            }
            else
            {
                throw new Exception("Chat message intention not supported");
            }
        }

        public async Task SendMessageToGameChat(string gameId, string username, string message, TypeOfMessage typeOfMessage = TypeOfMessage.Chat)
        {
            var user = _users.Find(x => x.ConnectionId == Context.ConnectionId);
            var game = _games.Find(x => x.GameSetup.Id == gameId);
            var allUsersInGame = GetPlayersAndSpectatorsFromGame(game);
            var chatMessageIntentionResult = GetChatMessageIntention(message);


            if (chatMessageIntentionResult.ChatMessageIntention == ChatMessageIntention.Buzz)
            {
                var targetedUser = _users.FirstOrDefault(x => x.Name == chatMessageIntentionResult.TargetedUsername);

                if (targetedUser != null)
                {
                    var canBeBuzzedAfter = targetedUser.LastBuzzedUtc.AddSeconds(Constants.MINIMUM_TIME_SECONDS_BETWEEN_BUZZ);
                    if (DateTime.Now > canBeBuzzedAfter)
                    {
                        targetedUser.LastBuzzedUtc=DateTime.Now;
                        await Clients.Client(targetedUser.ConnectionId).SendAsync("BuzzPlayer");
                        var msgDto = _mapper.Map<ChatMessageDto>(new ChatMessage("Server", $"User {user.Name} has buzzed player {targetedUser.Name}", TypeOfMessage.Server));
                        await Clients.Clients(allUsersInGame).SendAsync("PostNewMessageInGameChat", msgDto);
                    }
                    else
                    {
                        var msgDto = _mapper.Map<ChatMessageDto>(new ChatMessage("Server", $"User {targetedUser.Name} was not buzzed! Wait 5 seconds.", TypeOfMessage.Server));
                        await Clients.Caller.SendAsync("PostNewMessageInGameChat", msgDto);
                    }
                }
                else
                {
                    var msgDto = _mapper.Map<ChatMessageDto>(new ChatMessage("Server", $"Player {targetedUser.Name} not found", TypeOfMessage.Server));
                    await Clients.Caller.SendAsync("PostNewMessageInGameChat", msgDto);
                }
            }
            else if (chatMessageIntentionResult.ChatMessageIntention == ChatMessageIntention.Normal)
            {
                var msgDto = _mapper.Map<ChatMessageDto>(new ChatMessage(username, message, typeOfMessage));
                await Clients.Clients(allUsersInGame).SendAsync("PostNewMessageInGameChat", msgDto);
            }
            else
            {
                throw new Exception("Chat message intention not supported");
            }
        }

        public async Task SetGamePassword(string id, string password)
        {
            var game = _games.First(x => x.GameSetup.Id == id);
            game.GameSetup.Password = password;
            await GetAllGames();
            await GameUpdated(game);
            var message = string.IsNullOrEmpty(password) ? "Password Removed" : "Password updated";
            await DisplayToastMessageToGame(id, message);
        }

        public async Task GetAllOnlineUsers()
        {
            var usersDto = _mapper.Map<List<UserDto>>(_users);
            await Clients.All.SendAsync("RefreshOnlineUsersList", usersDto);
        }

        public async Task GetAllGames()
        {
            var gamesDtos = _mapper.Map<List<GameDto>>(_games);
            await Clients.All.SendAsync("RefreshAllGamesList", gamesDtos);
        }


        public async Task CreateGame(GameMode gameMode)
        {
            var user = _users.Find(x => x.ConnectionId == Context.ConnectionId);
            var gameSetup = new GameSetup(gameMode);
            var game = new Game(gameSetup);
            game.Players.Add(new Player(user));
            _games.Add(game);
            await GameUpdated(game);
            await GetAllGames();
            await SendMessageToAllChat("Server", $"User {user.Name} has created new game", TypeOfMessage.Server);
        }

        public async Task ExitGame(string gameid)
        {
            var game = _games.Find(x => x.GameSetup.Id == gameid);
            var user = _users.Find(x => x.ConnectionId == Context.ConnectionId);

            var allPlayersFromGame = GetPlayersFromGame(game);

            if (allPlayersFromGame.Contains(Context.ConnectionId))
            {
                var player = game.Players.Find(y => y.User.ConnectionId == Context.ConnectionId);
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
            else
            {
                game.Spectators.Remove(game.Spectators.Find(x => x.User.ConnectionId == Context.ConnectionId));
            }

            await GameUpdated(game);
            await SendMessageToGameChat(gameid, "Server", $"{user.Name} has left the game.", TypeOfMessage.Server);

            if (!game.Players.Any(x => x.LeftGame == false) && !game.Spectators.Any())
                _games.Remove(game);
            await GetAllGames();
        }

        public async Task KickPlayerFromGame(string name, string gameId)
        {
            var game = _games.Find(x => x.GameSetup.Id == gameId);

            var playerToKick = game.Players.Find(y => y.User.Name == name);

            game.Players.Remove(playerToKick);

            await GameUpdated(game);
            await GetAllGames();
            await Clients.Client(playerToKick.User.ConnectionId).SendAsync("KickPlayerFromGame");
        }


        public async Task StartGame(string gameId)
        {
            var game = _games.Find(x => x.GameSetup.Id == gameId);

            game.StartGame();

            await GameUpdated(game);
            await GetAllGames();
        }

        public async Task JoinGame(string gameId, string password)
        {
            await CleanupUserFromGamesExceptThisGame(gameId);
            var user = _users.Find(x => x.ConnectionId == Context.ConnectionId);
            var game = _games.Find(x => x.GameSetup.Id == gameId);

            var spectator = game.Spectators.FirstOrDefault(x => x.User == user);

            if (game.GameSetup.IsPasswordProtected && spectator == null)
                if (game.GameSetup.Password != password)
                    return;

            if (!game.GameStarted)
            {
                if (spectator != null)
                {
                    //join the gamt that hasn't started
                    game.Spectators.Remove(spectator);
                    game.Players.Add(new Player(user));
                }
                else
                {
                    //spectate game that hasn't started
                    game.Spectators.Add(new Spectator(user));
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
                    game.Spectators.Add(new Spectator(user));
                    await SendMessageToGameChat(gameId, "Server", $"{user.Name} has joined the game room.", TypeOfMessage.Server);
                }
            }


            await GameUpdated(game);
            await GetAllGames();
        }


        public void AddOrRenameUser(string name)
        {
            User user;
            lock (_users)
            {
                name = Regex.Replace(name, @"[^a-zA-Z0-9]", "").ToLower();

                if (name.Length > 10)
                    name = name.Substring(0, 10);
                var nameExists = _users.Any(x => x.Name == name);
                if (!nameExists)
                {
                    user = _users.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
                    if (user != null)
                    {
                        SendMessageToAllChat("Server", $"{user.Name} has renamed to {name}", TypeOfMessage.Server).ConfigureAwait(false);
                        user.Name = name;
                    }
                    else
                    {
                        user = new User(Context.ConnectionId, name);
                        _users.Add(user);
                        SendMessageToAllChat("Server", $"{user.Name} has connected to the server.", TypeOfMessage.Server).ConfigureAwait(false);
                    }
                    GetAllOnlineUsers().ConfigureAwait(false);
                    var userDto = _mapper.Map<UserDto>(user);
                    Clients.Client(Context.ConnectionId).SendAsync("UpdateCurrentUser", userDto).ConfigureAwait(false);
                }
                else
                {
                    Clients.Caller.SendAsync("RenamePlayer").ConfigureAwait(false);
                }
            }
        }

        public async Task DrawCard(string gameId, int count, bool normalDraw)
        {
            var user = _users.Find(x => x.ConnectionId == Context.ConnectionId);
            var game = _games.Find(x => x.GameSetup.Id == gameId);
            lock (game)
            {
                if (normalDraw)
                {
                    if (game.PlayerToPlay.User.Name == user.Name)
                    {
                        game.DrawCard(game.PlayerToPlay, count, normalDraw);
                        GameUpdated(game).ConfigureAwait(false);
                    }
                }
                else
                {
                    var player = game.Players.Find(x => x.User.Name == user.Name);
                    game.DrawCard(player, count, normalDraw);
                    GameUpdated(game).ConfigureAwait(false);
                }

            }
        }

        public async Task PlayCard(string gameId, CardDto cardDto, CardColor pickedCardColor, string targetedPlayerName)
        {
            var game = _games.Find(x => x.GameSetup.Id == gameId);
            var user = _users.Find(x => x.ConnectionId == Context.ConnectionId);
            lock (game)
            {
                if (game.GameEnded || !game.GameStarted)
                    return;
                var card = _mapper.Map<Card>(cardDto);
                var player = game.Players.Find(x => x.User.Name == user.Name);
                var successAndMessages = game.PlayCard(player, card, pickedCardColor, targetedPlayerName);
                if (successAndMessages.Key)
                {
                    if (cardDto.Value == CardValue.InspectHand)
                    {
                        var targetedPlayer = game.Players.Find(x => x.User.Name == targetedPlayerName);
                        Clients.Caller.SendAsync("ShowInspectedHand", _mapper.Map<HandDto>(targetedPlayer.Cards));
                        successAndMessages.Value.Add($"Player {player.User.Name} has inspected {targetedPlayer.User.Name}'s hand.");
                    }
                    else if (cardDto.Value == CardValue.GraveDigger)
                    {
                        Clients.Caller.SendAsync("ShowDiscardedPile", game.DiscardedPile);
                    }
                    GameUpdated(game).ConfigureAwait(false);
                }
                if (successAndMessages.Value.Any())
                {
                    successAndMessages.Value.ForEach(x => SendMessageToGameChat(game.GameSetup.Id, "Server", x, TypeOfMessage.Server).ConfigureAwait(false));
                }
            }
        }

        public async Task DigCardFromDiscardedPile(string gameId, CardDto cardDto)
        {
            var game = _games.Find(x => x.GameSetup.Id == gameId);
            var user = _users.Find(x => x.ConnectionId == Context.ConnectionId);

            var cardToPickUp = game.DiscardedPile.Find(x => x.Color == cardDto.Color && x.Value == cardDto.Value);

            game.Players.Find(x => x.User.Name == user.Name).Cards.Add(cardToPickUp);
            game.DiscardedPile.Remove(cardToPickUp);
            await GameUpdated(game);

        }



        //-------------------------- private

        private async Task DisplayToastMessageToGame(string gameid, string message)
        {
            var game = _games.Find(x => x.GameSetup.Id == gameid);
            var allUsersInGame = GetPlayersAndSpectatorsFromGame(game);

            await Clients.Clients(allUsersInGame).SendAsync("DisplayToastMessage", message);
        }

        private async Task GameUpdated(Game game)
        {
            var gameDto = _mapper.Map<GameDto>(game);

            var allUsersInGame = GetPlayersAndSpectatorsFromGame(game);
            await Clients.Clients(allUsersInGame).SendAsync("UpdateGame", gameDto);

            if (game.GameStarted)
            {
                var allPlayersInTheGame = GetPlayersFromGame(game);
                foreach (var connectionId in allPlayersInTheGame)
                {
                    var myHand = game.Players.FirstOrDefault(x => x.User.ConnectionId == connectionId).Cards;
                    var myHandDto = _mapper.Map<HandDto>(myHand);
                    await Clients.Client(connectionId).SendAsync("UpdateMyHand", myHandDto);
                }
            }
        }

        private List<string> GetPlayersFromGame(Game game)
        {
            return game.Players.Where(x => !x.LeftGame).Select(y => y.User.ConnectionId).ToList();
        }

        private List<string> GetPlayersAndSpectatorsFromGame(Game game)
        {
            return GetPlayersFromGame(game).Concat(game.Spectators.Select(x => x.User.ConnectionId)).ToList();
        }

        private async Task CleanupUserFromGames()
        {
            List<Game> games = _games.Where(x => GetPlayersAndSpectatorsFromGame(x).Where(y => y == Context.ConnectionId).Any()).ToList();

            foreach (var game in games)
            {
                await ExitGame(game.GameSetup.Id);
            }
        }

        private async Task CleanupUserFromGamesExceptThisGame(string gameId)
        {
            List<Game> games = _games.Where(x => x.GameSetup.Id != gameId && GetPlayersAndSpectatorsFromGame(x).Where(y => y == Context.ConnectionId).Any()).ToList();

            foreach (var game in games)
            {
                await ExitGame(game.GameSetup.Id);
            }
        }

        private async Task CleanupUserFromOnlineUsersList()
        {
            var user = _users.Find(x => x.ConnectionId == Context.ConnectionId);
            _users.Remove(user);
            await GetAllOnlineUsers();
        }

        private ChatMessageIntentionResult GetChatMessageIntention(string message)
        {
            Regex regex = new Regex(@"^/(slap|buzz|alert) ([A-Za-z0-9\s]*)$");
            Match match = regex.Match(message);

            if (match.Success)
            {
                var targetedUsername = match.Groups[2].Value;
                return new ChatMessageIntentionResult() { ChatMessageIntention = ChatMessageIntention.Buzz, TargetedUsername = targetedUsername };
            }
            else
            {
                return new ChatMessageIntentionResult() { ChatMessageIntention = ChatMessageIntention.Normal };
            }
        }

    }
}