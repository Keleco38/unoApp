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
using Uno.Models.Entities;
using Uno.Models.Helpers;

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

            await SendMessageToAllChat($"{user.Name} has left the server.", TypeOfMessage.Server);

            await CleanupUserFromGames();
            await CleanupUserFromOnlineUsersList();

            await base.OnDisconnectedAsync(exception);
        }
        public async Task SendMessageToAllChat(string message, TypeOfMessage typeOfMessage = TypeOfMessage.Chat)
        {
            var user = _users.Find(x => x.ConnectionId == Context.ConnectionId);
            var username = typeOfMessage == TypeOfMessage.Server ? "Server" : user.Name;

            var chatMessageIntentionResult = GetChatMessageIntention(message);

            if (chatMessageIntentionResult.ChatMessageIntention == ChatMessageIntention.Buzz)
            {
                var targetedUser = _users.FirstOrDefault(x => x.Name == chatMessageIntentionResult.TargetedUsername);
                if (targetedUser != null)
                {
                    var canBeBuzzedAfter = targetedUser.LastBuzzedUtc.AddSeconds(Constants.MINIMUM_TIME_SECONDS_BETWEEN_BUZZ);
                    if (DateTime.Now > canBeBuzzedAfter)
                    {
                        targetedUser.LastBuzzedUtc = DateTime.Now;
                        await Clients.Client(targetedUser.ConnectionId).SendAsync("BuzzPlayer", chatMessageIntentionResult.BuzzType);
                        var msgDto = _mapper.Map<ChatMessageDto>(new ChatMessage("Server", $"User {user.Name} has {chatMessageIntentionResult.BuzzTypeStringForChat} player {targetedUser.Name}", TypeOfMessage.Server));
                        await Clients.All.SendAsync("PostNewMessageInAllChat", msgDto);
                    }
                    else
                    {
                        var msgDto = _mapper.Map<ChatMessageDto>(new ChatMessage("Server", $"User {targetedUser.Name} was not {chatMessageIntentionResult.BuzzTypeStringForChat}! Wait {Constants.MINIMUM_TIME_SECONDS_BETWEEN_BUZZ} seconds.", TypeOfMessage.Server));
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

        public async Task SendMessageToGameChat(string gameId, string message, TypeOfMessage typeOfMessage = TypeOfMessage.Chat)
        {
            var user = _users.Find(x => x.ConnectionId == Context.ConnectionId);
            var username = typeOfMessage == TypeOfMessage.Server ? "Server" : user.Name;
            var game = _games.Find(x => x.Id == gameId);
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
                        targetedUser.LastBuzzedUtc = DateTime.Now;
                        await Clients.Client(targetedUser.ConnectionId).SendAsync("BuzzPlayer", chatMessageIntentionResult.BuzzType);
                        var msgDto = _mapper.Map<ChatMessageDto>(new ChatMessage("Server", $"User {user.Name} has {chatMessageIntentionResult.BuzzTypeStringForChat} player {targetedUser.Name}", TypeOfMessage.Server));
                        await Clients.Clients(allUsersInGame).SendAsync("PostNewMessageInGameChat", msgDto);
                    }
                    else
                    {
                        var msgDto = _mapper.Map<ChatMessageDto>(new ChatMessage("Server", $"User {targetedUser.Name} was not {chatMessageIntentionResult.BuzzTypeStringForChat}! Wait {Constants.MINIMUM_TIME_SECONDS_BETWEEN_BUZZ} seconds.", TypeOfMessage.Server));
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
            var game = _games.First(x => x.Id == id);
            game.GameSetup.Password = password;
            await GetAllGames();
            await UpdateGame(game);
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


        public async Task CreateGame()
        {
            var user = _users.Find(x => x.ConnectionId == Context.ConnectionId);
            var game = new Game();
            game.Players.Add(new Player(user));
            _games.Add(game);
            await UpdateGame(game);
            await GetAllGames();
            await SendMessageToAllChat($"User {user.Name} has created new game", TypeOfMessage.Server);
        }

        public async Task ExitGame(string gameid)
        {
            var game = _games.Find(x => x.Id == gameid);
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

            await UpdateGame(game);
            await SendMessageToGameChat(gameid, $"{user.Name} has left the game.", TypeOfMessage.Server);

            if (!game.Players.Any(x => x.LeftGame == false) && !game.Spectators.Any())
                _games.Remove(game);
            await GetAllGames();
        }

        public async Task KickPlayerFromGame(string name, string gameId)
        {
            var game = _games.Find(x => x.Id == gameId);

            var playerToKick = game.Players.Find(y => y.User.Name == name);

            game.Players.Remove(playerToKick);

            await UpdateGame(game);
            await GetAllGames();
            await Clients.Client(playerToKick.User.ConnectionId).SendAsync("KickPlayerFromGame");
        }

        public async Task UpdateGameSetup(string gameId, List<CardValue> bannedCards, int roundsToWin)
        {
            var game = _games.Find(x => x.Id == gameId);

            game.GameSetup.BannedCards = bannedCards;
            game.GameSetup.RoundsToWin = roundsToWin;

            await UpdateGame(game);
            await GetAllGames();
        }


        public async Task StartGame(string gameId)
        {
            var game = _games.Find(x => x.Id == gameId);

            game.StartNewGame();

            await UpdateGame(game);
            await UpdateHands(game);
            await GetAllGames();
            await AddToGameLog(gameId, "Game started!");
            await AddToGameLog(gameId, "If you need more detailed log info, press the 'Game info' button.");
            await AddToGameLog(gameId, "This is the game log summary. We will display the last 3 entries here.");
        }

        public async Task JoinGame(string gameId, string password)
        {
            await CleanupUserFromGamesExceptThisGame(gameId);
            var user = _users.Find(x => x.ConnectionId == Context.ConnectionId);
            var game = _games.Find(x => x.Id == gameId);

            var spectator = game.Spectators.FirstOrDefault(x => x.User == user);

            if (game.GameSetup.IsPasswordProtected && spectator == null)
                if (game.GameSetup.Password != password)
                    return;

            if (!game.GameStarted)
            {
                if (spectator != null)
                {
                    //join the game that hasn't started
                    game.Spectators.Remove(spectator);
                    game.Players.Add(new Player(user));
                }
                else
                {
                    //spectate game that hasn't started
                    game.Spectators.Add(new Spectator(user));
                    await SendMessageToGameChat(gameId, $"{user.Name} has joined the game room.", TypeOfMessage.Server);
                }
            }
            else
            {
                var playerLeftWithThisName = game.Players.FirstOrDefault(x => x.LeftGame && x.User.Name == user.Name);

                if (playerLeftWithThisName != null)
                {
                    playerLeftWithThisName.User = user;
                    playerLeftWithThisName.LeftGame = false;

                    await DisplayToastMessageToGame(gameId, $"PLAYER {user.Name} HAS RECONNECTED TO THE GAME");
                    await SendMessageToGameChat(gameId, $"{user.Name} has joined the game room.", TypeOfMessage.Server);
                    await UpdateHands(game);
                }
                else
                {
                    game.Spectators.Add(new Spectator(user));
                    await SendMessageToGameChat(gameId, $"{user.Name} has joined the game room.", TypeOfMessage.Server);
                }
            }

            await UpdateGame(game);
            await GetAllGames();
        }


        public async Task AddOrRenameUser(string name)
        {
            name = Regex.Replace(name, @"[^a-zA-Z0-9]", "").ToLower();

            if (!name.Any())
            {
                await Clients.Caller.SendAsync("RenamePlayer");
            }

            if (name.Length > 10)
                name = name.Substring(0, 10);
            var nameExists = _users.Any(x => x.Name == name);
            if (!nameExists && name != "server")
            {
                var existingUser = _users.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
                if (existingUser != null)
                {
                    await SendMessageToAllChat($"{existingUser.Name} has renamed to {name}", TypeOfMessage.Server);
                    _users.Remove(existingUser);
                }
                else
                {
                    await SendMessageToAllChat($"{name} has connected to the server.", TypeOfMessage.Server);
                }
                var user = new User(Context.ConnectionId, name);
                _users.Add(user);
                var userDto = _mapper.Map<UserDto>(user);
                await Clients.Client(Context.ConnectionId).SendAsync("UpdateCurrentUser", userDto);
                await GetAllOnlineUsers();
            }
            else
            {
                await Clients.Caller.SendAsync("RenamePlayer");
            }
        }

        public async Task DrawCard(string gameId, int count, bool normalDraw)
        {
            var user = _users.Find(x => x.ConnectionId == Context.ConnectionId);
            var game = _games.Find(x => x.Id == gameId);
            if (normalDraw)
            {
                if (game.PlayerToPlay.User.Name == user.Name)
                {
                    game.DrawCard(game.PlayerToPlay, count, normalDraw);
                    await AddToGameLog(gameId, $"{user.Name} drew a card (normal draw)");
                }
            }
            else
            {
                var player = game.Players.Find(x => x.User.Name == user.Name);
                game.DrawCard(player, count, normalDraw);
            }
            await UpdateGame(game);
            await UpdateHands(game);

        }

        public async Task PlayCard(string gameId, string cardPlayedId, CardColor targetedCardColor, string playerTargetedId, string cardToDigId, List<int> duelNumbers, List<string> charityCardsIds, int blackjackNumber, List<int> numbersToDiscard)
        {
            var game = _games.Find(x => x.Id == gameId);
            if (game.GameEnded || !game.GameStarted)
                return;
            var user = _users.Find(x => x.ConnectionId == Context.ConnectionId);
            var player = game.Players.Find(x => x.User.Name == user.Name);
            var moveResult = game.PlayCard(player, cardPlayedId, targetedCardColor, playerTargetedId, cardToDigId, duelNumbers, charityCardsIds, blackjackNumber, numbersToDiscard);

            if (moveResult == null)
                return;

            moveResult.MoveResultCallbackParams.ForEach(async callbackParam =>
            {
                await Clients.Client(callbackParam.ConnectionId).SendAsync(callbackParam.Command, callbackParam.Object);
            });
            moveResult.MessagesToLog.ForEach(async x => await AddToGameLog(game.Id, x));

            await UpdateGame(game);
            await UpdateHands(game);

            if (player.Cards.Count == 1)
            {
                await Clients.Caller.SendAsync("MustCallUno");
            }

        }
        //-------------------------- private

        private async Task AddToGameLog(string gameid, string message)
        {
            var game = _games.Find(x => x.Id == gameid);
            var allUsersInGame = GetPlayersAndSpectatorsFromGame(game);

            await Clients.Clients(allUsersInGame).SendAsync("AddToGameLog", message);
        }

        private async Task DisplayToastMessageToGame(string gameid, string message)
        {
            var game = _games.Find(x => x.Id == gameid);
            var allUsersInGame = GetPlayersAndSpectatorsFromGame(game);

            await Clients.Clients(allUsersInGame).SendAsync("DisplayToastMessage", message);
        }

        private async Task UpdateGame(Game game)
        {
            var gameDto = _mapper.Map<GameDto>(game);

            var allUsersInGame = GetPlayersAndSpectatorsFromGame(game);
            await Clients.Clients(allUsersInGame).SendAsync("UpdateGame", gameDto);

        }
        private async Task UpdateHands(Game game)
        {
            if (game.GameStarted)
            {
                var allPlayersInTheGame = GetPlayersFromGame(game);
                foreach (var connectionId in allPlayersInTheGame)
                {
                    var myCards = game.Players.FirstOrDefault(x => x.User.ConnectionId == connectionId).Cards;
                    var myCardsDto = _mapper.Map<List<CardDto>>(myCards);
                    await Clients.Client(connectionId).SendAsync("UpdateMyHand", myCardsDto);
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
                await ExitGame(game.Id);
            }
        }

        private async Task CleanupUserFromGamesExceptThisGame(string gameId)
        {
            List<Game> games = _games.Where(x => x.Id != gameId && GetPlayersAndSpectatorsFromGame(x).Where(y => y == Context.ConnectionId).Any()).ToList();

            foreach (var game in games)
            {
                await ExitGame(game.Id);
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
            Regex regex = new Regex(@"^/(slap|ding|alert|lick|poke|punch) ([A-Za-z0-9\s]*)$");
            Match match = regex.Match(message);

            if (match.Success)
            {
                var targetedUsername = match.Groups[2].Value;
                var buzzType = match.Groups[1].Value;
                var buzzTypeStringForChat = string.Empty; ;
                switch (buzzType)
                {
                    case "slap":
                        buzzTypeStringForChat = "slapped";
                        break;
                    case "ding":
                        buzzTypeStringForChat = "dinged";
                        break;
                    case "alert":
                        buzzTypeStringForChat = "alerted";
                        break;
                    case "lick":
                        buzzTypeStringForChat = "licked";
                        break;
                    case "poke":
                        buzzTypeStringForChat = "poked";
                        break;
                    case "punch":
                        buzzTypeStringForChat = "punched";
                        break;
                }
                return new ChatMessageIntentionResult() { ChatMessageIntention = ChatMessageIntention.Buzz, TargetedUsername = targetedUsername, BuzzType = buzzType, BuzzTypeStringForChat = buzzTypeStringForChat };
            }
            else
            {
                return new ChatMessageIntentionResult() { ChatMessageIntention = ChatMessageIntention.Normal };
            }
        }

    }
}