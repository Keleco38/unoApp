using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.Internal;
using Uno.Contants;
using Uno.Enums;
using Uno.Helpers;
using Uno.Models;
using Uno.Models.Dtos;
using Uno.Models.Entities;
using Uno.Models.Helpers;

namespace Uno.Hubs
{
    public class GameHub : Hub
    {
        private static readonly SemaphoreLocker _gameLocker = new SemaphoreLocker();
        private static readonly SemaphoreLocker _userLocker = new SemaphoreLocker();
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
            var user = GetUserByConnectionId();
            await SendMessage($"{user.Name} has left the server.", TypeOfMessage.Server);
            await CleanupUserFromGames();
            await CleanupUserFromOnlineUsersList();
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string message, string gameId = "")
        {
            if (!string.IsNullOrWhiteSpace(gameId))
            {
                var game = GetGameByGameId(gameId);
                var isPlayer = GetPlayersFromGame(game).FirstOrDefault(x => x == Context.ConnectionId) != null;
                await SendMessage(message, isPlayer ? TypeOfMessage.Chat : TypeOfMessage.Spectators, gameId);
            }
            else
            {
                await SendMessage(message, TypeOfMessage.Chat);
            }
        }

        public async Task SetGamePassword(string gameId, string password)
        {
            var game = GetGameByGameId(gameId);
            game.GameSetup.Password = password;
            await GetAllGames();
            await UpdateGame(game);
        }

        public async Task GetAllOnlineUsers()
        {
            var usersDto = _mapper.Map<List<UserDto>>(_users);
            await Clients.All.SendAsync("RefreshOnlineUsersList", usersDto);
        }

        public async Task GetAllGames()
        {
            var gamesDto = _mapper.Map<List<GameDto>>(_games);
            await Clients.All.SendAsync("RefreshAllGamesList", gamesDto);
        }

        public async Task CreateGame()
        {
            var user = GetUserByConnectionId();
            var game = new Game();
            game.Players.Add(new Player(user));
            _games.Add(game);
            await UpdateGame(game);
            await GetAllGames();
            await SendMessage($"User {user.Name} has created new game", TypeOfMessage.Server);
        }

        public async Task ExitGame(string gameId)
        {
            var game = GetGameByGameId(gameId);
            var user = GetUserByConnectionId();
            var allPlayersFromGame = GetPlayersFromGame(game);
            if (allPlayersFromGame.Contains(Context.ConnectionId))
            {
                var player = game.Players.Find(y => y.User.ConnectionId == Context.ConnectionId);
                if (game.GameStarted)
                {
                    player.LeftGame = true;
                    await DisplayToastMessageToGame(gameId, $"User {player.User.Name} has left the game.");
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
            await SendMessage($"{user.Name} has left the game.", TypeOfMessage.Server, gameId);
            if (game.Players.All(x => x.LeftGame) && !game.Spectators.Any())
            {
                _games.Remove(game);
            }
            await GetAllGames();
            await Clients.Caller.SendAsync("ExitGame");
        }

        public async Task KickPlayerFromGame(string name, string gameId)
        {
            var game = GetGameByGameId(gameId);
            var playerToKick = game.Players.Find(y => y.User.Name == name);
            game.Players.Remove(playerToKick);
            await UpdateGame(game);
            await GetAllGames();
            await Clients.Client(playerToKick.User.ConnectionId).SendAsync("KickPlayerFromGame");
        }

        public async Task UpdateGameSetup(string gameId, List<CardValue> bannedCards, int roundsToWin)
        {
            var game = GetGameByGameId(gameId);
            game.GameSetup.BannedCards = bannedCards;
            game.GameSetup.RoundsToWin = roundsToWin;
            await UpdateGame(game);
            await GetAllGames();
        }

        public async Task StartGame(string gameId)
        {
            var game = GetGameByGameId(gameId);
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
            var user = GetUserByConnectionId();
            var game = GetGameByGameId(gameId);
            var spectator = game.Spectators.FirstOrDefault(x => x.User == user);
            if (game.GameSetup.IsPasswordProtected && spectator == null)
                if (game.GameSetup.Password != password)
                {
                    await DisplayToastMessageToUser(user.ConnectionId, "Incorrect password.");
                    return;
                }
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
                    await SendMessage($"{user.Name} has joined the game room.", TypeOfMessage.Server, gameId);
                }
            }
            else
            {
                var playerLeftWithThisName = game.Players.FirstOrDefault(x => x.LeftGame && x.User.Name == user.Name);
                if (playerLeftWithThisName != null)
                {
                    playerLeftWithThisName.User = user;
                    playerLeftWithThisName.LeftGame = false;
                    await DisplayToastMessageToGame(gameId, $"Player {user.Name} has reconnected to the game.");
                    await SendMessage($"{user.Name} has joined the game room.", TypeOfMessage.Server, gameId);
                    await UpdateHands(game);
                }
                else
                {
                    game.Spectators.Add(new Spectator(user));
                    await SendMessage($"{user.Name} has joined the game room.", TypeOfMessage.Server, gameId);
                }
            }
            await UpdateGame(game);
            await GetAllGames();
        }

        public async Task AddOrRenameUser(string name)
        {
            await _userLocker.LockAsync(async () =>
            {
                name = Regex.Replace(name, @"[^a-zA-Z0-9]", "").ToLower();
                if (!name.Any())
                {
                    await Clients.Caller.SendAsync("RenamePlayer");
                }
                if (name.Length > 10)
                {
                    name = name.Substring(0, 10);
                }
                var nameExists = _users.Any(x => x.Name == name);
                if (!nameExists && name != "server")
                {
                    string message = string.Empty;
                    var existingUser = GetUserByConnectionId();
                    if (existingUser != null)
                    {
                        message = $"{existingUser.Name} has renamed to {name}";
                        _users.Remove(existingUser);
                    }
                    else
                    {
                        message = $"{name} has connected to the server.";
                    }
                    var user = new User(Context.ConnectionId, name);
                    _users.Add(user);
                    await SendMessage(message, TypeOfMessage.Server);
                    var userDto = _mapper.Map<UserDto>(user);
                    await Clients.Client(Context.ConnectionId).SendAsync("UpdateCurrentUser", userDto);
                    await GetAllOnlineUsers();
                }
                else
                {
                    await Clients.Caller.SendAsync("RenamePlayer");
                }
            });
        }

        public async Task DrawCard(string gameId, int count, bool normalDraw)
        {
            var user = GetUserByConnectionId();
            var game = GetGameByGameId(gameId);
            if (game.GameEnded)
            {
                return;
            }
            if (normalDraw)
            {
                if (game.PlayerToPlay.User.Name == user.Name)
                {
                    if (game.PlayerToPlay.CardPromisedToDiscard != null)
                    {
                        game.DrawCard(game.PlayerToPlay, 2, false);
                        game.PlayerToPlay.CardPromisedToDiscard = null;
                        await AddToGameLog(gameId, $"Player didn't fulfill his promise, he will draw 2 cards. ");
                    }

                    game.DrawCard(game.PlayerToPlay, count, true);
                    await AddToGameLog(gameId, $"{user.Name} drew a card (normal draw)");
                }
            }
            else
            {
                var player = game.Players.Find(x => x.User.Name == user.Name);
                game.DrawCard(player, count, false);
            }
            await UpdateGame(game);
            await UpdateHands(game);
        }

        public async Task PlayCard(string gameId, string cardPlayedId, CardColor targetedCardColor, string playerTargetedId, string cardToDigId, List<int> duelNumbers, List<string> charityCardsIds, int blackjackNumber, List<int> numbersToDiscard, string cardPromisedToDiscardId, string oddOrEvenGuess)
        {

            await _gameLocker.LockAsync(async () =>
            {
                var game = GetGameByGameId(gameId);
                if (game.GameEnded || !game.GameStarted)
                    return;
                var user = GetUserByConnectionId();
                var player = game.Players.Find(x => x.User.Name == user.Name);
                var moveResult = game.PlayCard(player, cardPlayedId, targetedCardColor, playerTargetedId, cardToDigId, duelNumbers, charityCardsIds, blackjackNumber, numbersToDiscard, cardPromisedToDiscardId, oddOrEvenGuess);
                if (moveResult == null)
                {
                    return;
                }
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
            });

        }

        #region private

        private async Task AddToGameLog(string gameId, string message)
        {
            var game = GetGameByGameId(gameId);
            var allUsersInGame = GetPlayersAndSpectatorsFromGame(game);
            await Clients.Clients(allUsersInGame).SendAsync("AddToGameLog", message);
        }

        private async Task DisplayToastMessageToGame(string gameid, string message)
        {
            var game = _games.Find(x => x.Id == gameid);
            var allUsersInGame = GetPlayersAndSpectatorsFromGame(game);
            await Clients.Clients(allUsersInGame).SendAsync("DisplayToastMessage", message);
        }

        private async Task DisplayToastMessageToUser(string connectionId, string message)
        {
            await Clients.Client(connectionId).SendAsync("DisplayToastMessage", message);
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
                    var myCards = game.Players.Find(x => x.User.ConnectionId == connectionId).Cards;
                    var myCardsDto = _mapper.Map<List<CardDto>>(myCards).OrderBy(x => x.Color).ThenBy(x => x.Value);
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
            List<Game> games = _games.Where(x => GetPlayersAndSpectatorsFromGame(x).Any(y => y == Context.ConnectionId)).ToList();
            foreach (var game in games)
            {
                await ExitGame(game.Id);
            }
        }

        private async Task CleanupUserFromGamesExceptThisGame(string gameId)
        {
            List<Game> games = _games.Where(x => x.Id != gameId && GetPlayersAndSpectatorsFromGame(x).Any(y => y == Context.ConnectionId)).ToList();
            foreach (var game in games)
            {
                await ExitGame(game.Id);
            }
        }

        private async Task CleanupUserFromOnlineUsersList()
        {
            var user = GetUserByConnectionId();
            _users.Remove(user);
            await GetAllOnlineUsers();
        }

        private async Task SendMessage(string message, TypeOfMessage typeOfMessage, string gameId = "")
        {
            var user = GetUserByConnectionId();
            var username = typeOfMessage == TypeOfMessage.Server ? "Server" : user.Name;
            var chatMessageIntentionResult = GetChatMessageIntention(message);
            ChatMessageDto msgDto;
            var allUsersInGame = new List<string>();
            bool buzzFailed = false;
            if (!string.IsNullOrWhiteSpace(gameId))
            {
                var game = GetGameByGameId(gameId);
                allUsersInGame = GetPlayersAndSpectatorsFromGame(game);
            }
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
                        msgDto = _mapper.Map<ChatMessageDto>(new ChatMessage("Server", $"User {user.Name} has {chatMessageIntentionResult.BuzzTypeStringForChat} user {targetedUser.Name}", TypeOfMessage.Server));
                    }
                    else
                    {
                        msgDto = _mapper.Map<ChatMessageDto>(new ChatMessage("Server", $"User {chatMessageIntentionResult.TargetedUsername} was not {chatMessageIntentionResult.BuzzTypeStringForChat}! Wait {Constants.MINIMUM_TIME_SECONDS_BETWEEN_BUZZ} seconds.", TypeOfMessage.Server));
                        buzzFailed = true;
                    }
                }
                else
                {
                    msgDto = _mapper.Map<ChatMessageDto>(new ChatMessage("Server", $"User {chatMessageIntentionResult.TargetedUsername} not found", TypeOfMessage.Server));
                    buzzFailed = true;
                }

                if (buzzFailed)
                {
                    await Clients.Caller.SendAsync("PostNewMessageInGameChat", msgDto);
                    await Clients.Caller.SendAsync("PostNewMessageInAllChat", msgDto);
                }
                else
                {
                    await Clients.Clients(allUsersInGame).SendAsync("PostNewMessageInGameChat", msgDto);
                    await Clients.All.SendAsync("PostNewMessageInAllChat", msgDto);
                }

            }
            else if (chatMessageIntentionResult.ChatMessageIntention == ChatMessageIntention.Normal)
            {
                msgDto = _mapper.Map<ChatMessageDto>(new ChatMessage(username, message, typeOfMessage));
                if (!string.IsNullOrWhiteSpace(gameId))
                {
                    await Clients.Clients(allUsersInGame).SendAsync("PostNewMessageInGameChat", msgDto);
                }
                else
                {
                    await Clients.All.SendAsync("PostNewMessageInAllChat", msgDto);
                }

                chatMessageIntentionResult.MentionedUsers.ForEach(async targetedUser =>
                {
                    var canBeBuzzedAfter = targetedUser.LastBuzzedUtc.AddSeconds(Constants.MINIMUM_TIME_SECONDS_BETWEEN_BUZZ);
                    if (DateTime.Now > canBeBuzzedAfter)
                    {
                        targetedUser.LastBuzzedUtc = DateTime.Now;
                        await Clients.Client(targetedUser.ConnectionId).SendAsync("UserMentioned");
                    }
                });
            }
        }

        private ChatMessageIntentionResult GetChatMessageIntention(string message)
        {
            Regex regex = new Regex(@"^/(slap|ding|alert|lick|poke|punch|shoot|scream|laugh|kiss) @?([A-Za-z0-9\s]*)$");
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
                    case "shoot":
                        buzzTypeStringForChat = "shot";
                        break;
                    case "scream":
                        buzzTypeStringForChat = "screamed at";
                        break;
                    case "laugh":
                        buzzTypeStringForChat = "laughed at";
                        break;
                    case "kiss":
                        buzzTypeStringForChat = "kissed";
                        break;
                }
                return new ChatMessageIntentionResult() { ChatMessageIntention = ChatMessageIntention.Buzz, TargetedUsername = targetedUsername, BuzzType = buzzType, BuzzTypeStringForChat = buzzTypeStringForChat };
            }
            else
            {
                var mentionedUsers = new List<User>();

                List<Match> matches = Regex.Matches(message, @"@([A-Za-z0-9]*)").ToList();
                matches.ForEach(x =>
                {
                    var username = x.Groups[1].Value;

                    var user = _users.FirstOrDefault(y => y.Name == username);
                    if (user != null && !mentionedUsers.Contains(user))
                    {
                        mentionedUsers.Add(user);
                    }
                });

                return new ChatMessageIntentionResult() { ChatMessageIntention = ChatMessageIntention.Normal, MentionedUsers = mentionedUsers };
            }
        }

        private User GetUserByConnectionId()
        {
            var user = _users.Find(x => x.ConnectionId == Context.ConnectionId);
            return user;
        }

        private Game GetGameByGameId(string gameId)
        {
            var game = _games.Find(x => x.Id == gameId);
            return game;

        }

        #endregion

    }
}