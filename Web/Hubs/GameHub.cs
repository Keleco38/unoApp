using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using Common.Contants;
using Common.Enums;
using DomainObjects;
using EntityObjects;
using GameProcessingService.CoreManagers;
using GameProcessingService.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Repository;
using Web.Helpers;
using Web.Models;

namespace Web.Hubs
{
    public class GameHub : Hub
    {
        private readonly IMapper _mapper;
        private readonly IGameManager _gameManager;
        private readonly IPlayCardManager _playCardManager;
        private readonly IUserRepository _userRepository;
        private readonly IGameRepository _gameRepository;
        private readonly IHallOfFameRepository _hallOfFameRepository;
        private readonly ITournamentRepository _tournamentRepository;
        private readonly ITournamentManager _tournamentManager;
        private AppSettings _appSettings;

        public GameHub(IMapper mapper, IGameManager gameManager, IPlayCardManager playCardManager, IUserRepository userRepository, IGameRepository gameRepository, IHallOfFameRepository hallOfFameRepository, ITournamentRepository tournamentRepository, ITournamentManager tournamentManager, IOptions<AppSettings> appSettings)
        {
            _gameManager = gameManager;
            _playCardManager = playCardManager;
            _userRepository = userRepository;
            _gameRepository = gameRepository;
            _hallOfFameRepository = hallOfFameRepository;
            _tournamentRepository = tournamentRepository;
            _tournamentManager = tournamentManager;
            _appSettings = appSettings.Value;
            _mapper = mapper;
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(System.Exception exception)
        {
            if (_userRepository.UserExistsByConnectionId(Context.ConnectionId))
            {
                var user = GetCurrentUser();
                await SendMessage($"{user.Name} has left the server.", TypeOfMessage.Server, ChatDestination.All, string.Empty, string.Empty);
                await CleanupUserFromGames(user);
                await CleanupUserFromTournaments(user);
                await CleanupUserFromOnlineUsersList(user);
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string message, ChatDestination chatDestination, string gameId, string tournamentId)
        {
            var isPlayer = true;

            if (chatDestination == ChatDestination.Game)
            {
                var game = _gameRepository.GetGameByGameId(gameId);
                isPlayer = GetPlayersFromGame(game).FirstOrDefault(x => x == Context.ConnectionId) != null;
            }

            else if (chatDestination == ChatDestination.Tournament)
            {
                var tournament = _tournamentRepository.GetTournament(tournamentId);
                isPlayer = GetContestantsFromTournament(tournament).FirstOrDefault(x => x == Context.ConnectionId) != null;
            }

            await SendMessage(message, isPlayer ? TypeOfMessage.Chat : TypeOfMessage.Spectators, chatDestination, gameId, tournamentId);
        }


        public async Task GetAllOnlineUsers()
        {
            var usersDto = _mapper.Map<List<UserDto>>(_userRepository.GetAllUsers());
            await Clients.All.SendAsync("RefreshOnlineUsersList", usersDto);
        }

        public async Task GetAllGames()
        {
            var gamesDto = _mapper.Map<List<GameListDto>>(_gameRepository.GetAllGames().Where(x => !x.IsTournamentGame));
            await Clients.All.SendAsync("RefreshAllGamesList", gamesDto);
        }

        public async Task GetAllTournaments()
        {
            var tournamentsDto = _mapper.Map<List<TournamentListDto>>(_tournamentRepository.GetAllTournaments());
            await Clients.All.SendAsync("RefreshAllTournamentsList", tournamentsDto);
        }

        public async Task JoinTournament(string tournamentId, string password)
        {
            var user = GetCurrentUser();
            var tournament = _tournamentRepository.GetTournament(tournamentId);
            var spectator = tournament.Spectators.FirstOrDefault(x => x.Name == user.Name);
            if (!string.IsNullOrEmpty(tournament.TournamentSetup.Password) && spectator == null)
                if (tournament.TournamentSetup.Password != password)
                {
                    await DisplayToastMessageToUser(user.ConnectionId, "Incorrect password.");
                    return;
                }
            if (!tournament.TournamentStarted)
            {
                if (spectator != null)
                {
                    //join the game that hasn't started
                    if (tournament.Contestants.Count >= tournament.TournamentSetup.NumberOfPlayers)
                    {
                        return;
                    }
                    tournament.Spectators.Remove(spectator);
                    tournament.Contestants.Add(new Contestant(user));
                }
                else
                {
                    //spectate game that hasn't started
                    tournament.Spectators.Add(user);
                    await SendMessage($"{user.Name} has joined the tournament.", TypeOfMessage.Server, ChatDestination.Tournament, string.Empty, tournamentId);
                }
            }
            else
            {
                var contestant = tournament.Contestants.FirstOrDefault(x => x.User.Name == user.Name);
                if (contestant != null)
                {
                    contestant.User = user;
                    contestant.LeftTournament = false;
                    await SendMessage($"{user.Name} has joined the tournament.", TypeOfMessage.Server, ChatDestination.Tournament, string.Empty, tournamentId);
                }
                else
                {
                    tournament.Spectators.Add((user));
                    await SendMessage($"{user.Name} has joined the tournament.", TypeOfMessage.Server, ChatDestination.Tournament, string.Empty, tournamentId);
                }
            }

            var chatMsgs = tournament.ChatMessages.ToList();
            chatMsgs.Reverse();
            await Clients.Caller.SendAsync("RetrieveFullTournamentChat", _mapper.Map<List<ChatMessageDto>>(chatMsgs));
            await UpdateTournament(tournament);
            await GetAllTournaments();
        }

        public async Task CreateTournament(TournamentSetup tournamentSetup, string password)
        {
            if (string.IsNullOrEmpty(password) || !string.Equals(password, _appSettings.AdminPassword))
            {
                await DisplayToastMessageToUser(Context.ConnectionId, "Unauthorized");
                return;
            }

            if (string.IsNullOrEmpty(tournamentSetup.Name))
            {
                tournamentSetup.Name = "tournament";
            }

            else if (tournamentSetup.Name.Length > 10)
            {
                tournamentSetup.Name = tournamentSetup.Name.Substring(0, 10);

            }

            var tournament = new Tournament(tournamentSetup);
            tournament.Contestants.Add(new Contestant(GetCurrentUser()));
            _tournamentRepository.AddTournament(tournament);
            await UpdateTournament(tournament);
            await GetAllTournaments();
        }

        public async Task StartTournament(string tournamentId)
        {
            var user = GetCurrentUser();
            var tournament = _tournamentRepository.GetTournament(tournamentId);
            if (tournament.TournamentStarted || tournament.Contestants.First().User != user)
                return;
            if (tournament.Contestants.Count < 3)
                return;
            _tournamentManager.StartTournament(tournament);
            await UpdateTournament(tournament);
            await GetAllTournaments();
        }

        public async Task ExitTournament(string tournamentId)
        {
            var user = GetCurrentUser();
            await ExitTournament(tournamentId, user);
        }



        public async Task CreateGame(GameSetupDto gameSetupDto)
        {
            var user = GetCurrentUser();
            var gameSetup = _mapper.Map<GameSetup>(gameSetupDto);
            var game = new Game(gameSetup);
            game.Players.Add(new Player(user, game.Players.Count + 1));
            _gameRepository.AddGame(game);
            await UpdateGame(game);
            await GetAllGames();
            await SendMessage($"User {user.Name} has created new game", TypeOfMessage.Server, ChatDestination.All, string.Empty, string.Empty);
        }

        public async Task AdminKickUser(string name, string password)
        {
            if (string.IsNullOrEmpty(password) || !string.Equals(password, _appSettings.AdminPassword))
            {
                await DisplayToastMessageToUser(Context.ConnectionId, "Unauthorized");
                return;
            }

            if (_userRepository.UserExistsByName(name))
            {
                var user = _userRepository.GetUserByName(name);
                await SendMessage($"{user.Name} has left the server.", TypeOfMessage.Server, ChatDestination.All, string.Empty, string.Empty);
                await CleanupUserFromGames(user);
                await CleanupUserFromTournaments(user);
                await CleanupUserFromOnlineUsersList(user);
                await Clients.Client(user.ConnectionId).SendAsync("AdminKickUser");
            }
        }

        public async Task ExitGame(string gameId)
        {
            var user = GetCurrentUser();
            await ExitGame(gameId, user);
        }

        public async Task ChangeTeam(string gameId, int teamNumber)
        {
            if (teamNumber < 1 || teamNumber > 5)
                return;

            var game = _gameRepository.GetGameByGameId(gameId);
            if (game.GameSetup.PlayersSetup != PlayersSetup.Teams)
                return;
            var user = GetCurrentUser();
            var player = game.Players.First(x => x.User == user);
            player.TeamNumber = teamNumber;
            await UpdateGame(game);
        }

        public async Task KickPlayerFromGame(string name, string gameId)
        {
            var game = _gameRepository.GetGameByGameId(gameId);
            if (!Context.ConnectionId.Equals(game.Players.First().User.ConnectionId))
            {
                return;
            }

            var playerToKick = game.Players.First(y => y.User.Name == name);
            await Clients.Client(playerToKick.User.ConnectionId).SendAsync("KickPlayerFromGame");
        }
        public async Task KickContestantFromTournament(string name, string tournamentId)
        {
            var tournament = _tournamentRepository.GetTournament(tournamentId);
            if (!Context.ConnectionId.Equals(tournament.Contestants.First().User.ConnectionId))
            {
                return;
            }

            var playerToKick = tournament.Contestants.First(y => y.User.Name == name);
            await Clients.Client(playerToKick.User.ConnectionId).SendAsync("KickContestantFromTournament");
        }

        public async Task UpdateGameSetup(string gameId, GameSetupDto gameSetupDto)
        {
            var game = _gameRepository.GetGameByGameId(gameId);
            if (!Context.ConnectionId.Equals(game.Players.First().User.ConnectionId) || game.GameStarted)
            {
                return;
            }

            var gameSetup = _mapper.Map<GameSetup>(gameSetupDto);
            game.GameSetup = gameSetup;
            await UpdateGame(game);
            await GetAllGames();
        }
        public async Task UpdateTournamentSetup(string tournamentId, TournamentSetupDto tournamentSetupDto)
        {
            var tournament = _tournamentRepository.GetTournament(tournamentId);
            if (!Context.ConnectionId.Equals(tournament.Contestants.First().User.ConnectionId) || tournament.TournamentStarted)
            {
                return;
            }

            var tournamentSetup = _mapper.Map<TournamentSetup>(tournamentSetupDto);

            if (string.IsNullOrEmpty(tournamentSetup.Name))
            {
                tournamentSetup.Name = "unnamed";
            }

            else if (tournamentSetup.Name.Length > 10)
            {
                tournamentSetup.Name = tournamentSetup.Name.Substring(0, 10);

            }

            tournament.TournamentSetup = tournamentSetup;
            await UpdateTournament(tournament);
            await GetAllTournaments();
        }



        public async Task StartGame(string gameId)
        {
            var game = _gameRepository.GetGameByGameId(gameId);
            if (!Context.ConnectionId.Equals(game.Players.First().User.ConnectionId))
            {
                return;
            }

            _gameManager.StartNewGame(game);
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
            var user = GetCurrentUser();
            var game = _gameRepository.GetGameByGameId(gameId);

            if (game.IsTournamentGame && !game.GameStarted)
                return;


            bool alreadyAuthorized = false;
            if (game.IsTournamentGame)
            {
                var tournament = _tournamentRepository.GetTournament(game.TournamentId);
                alreadyAuthorized = tournament.Contestants.FirstOrDefault(x => x.User.Name == user.Name) != null;
                if (!alreadyAuthorized)
                    alreadyAuthorized = tournament.Spectators.FirstOrDefault(x => x.Name == user.Name) != null;
            }
            else
            {
                alreadyAuthorized = game.Spectators.FirstOrDefault(x => x.User.Name == user.Name) != null;

            }

            if (!string.IsNullOrEmpty(game.GameSetup.Password) && !alreadyAuthorized)
                if (game.GameSetup.Password != password)
                {
                    await DisplayToastMessageToUser(user.ConnectionId, "Incorrect password.");
                    return;
                }

            var spectator = game.Spectators.FirstOrDefault(x => x.User.Name == user.Name);

            if (!game.GameStarted)
            {
                if (spectator != null)
                {
                    //join the game that hasn't started
                    if (game.Players.Count >= game.GameSetup.MaxNumberOfPlayers)
                    {
                        return;
                    }
                    game.Spectators.Remove(spectator);
                    game.Players.Add(new Player(user, game.Players.Count + 1));
                }
                else
                {
                    //spectate game that hasn't started
                    game.Spectators.Add(new Spectator(user));
                    await SendMessage($"{user.Name} has joined the game room.", TypeOfMessage.Server, ChatDestination.Game, gameId, string.Empty);
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

                }
                else
                {
                    game.Spectators.Add(new Spectator(user));
                }
                await SendMessage($"{user.Name} has joined the game room.", TypeOfMessage.Server, ChatDestination.Game, gameId, string.Empty);
            }
            var chatMsgs = game.ChatMessages.ToList();
            chatMsgs.Reverse();
            var logs = game.GameLog.ToList();
            logs.Reverse();

            await Clients.Caller.SendAsync("RetrieveFullGameChat", _mapper.Map<List<ChatMessageDto>>(chatMsgs));
            await Clients.Caller.SendAsync("RetrieveFullGameLog", logs);
            await UpdateHands(game);
            await UpdateGame(game);
            await GetAllGames();
        }

        public async Task AddOrRenameUser(string name)
        {
            name = Regex.Replace(name, @"[^a-zA-Z0-9]", "").ToLower();

            if (name.Length > 10)
            {
                name = name.Substring(0, 10);
            }

            if (name == "server" || string.IsNullOrEmpty(name))
            {
                await Clients.Caller.SendAsync("RenamePlayer");
                return;
            }

            var nameExists = _userRepository.UserExistsByName(name);
            if (nameExists)
            {
                var connId = _userRepository.GetUserByName(name).ConnectionId;
                if (connId != Context.ConnectionId)
                {
                    await Clients.Caller.SendAsync("RenamePlayer");
                }
                return;
            }

            string message;
            User user;

            var userExists = _userRepository.UserExistsByConnectionId(Context.ConnectionId);
            if (userExists)
            {
                user = GetCurrentUser();
                message = $"{user.Name} has renamed to {name}";
                user.Name = name;
            }
            else
            {
                message = $"{name} has connected to the server.";
                user = new User(Context.ConnectionId, name);
                _userRepository.AddUser(user);
                ChatMessageDto msg = new ChatMessageDto() { CreatedUtc = DateTime.Now, Text = "If you need to create a tournament please contract one of the moderators (discord link in the navbar)", TypeOfMessage = TypeOfMessage.Server, Username = "Server" };
                await Clients.Caller.SendAsync("PostNewMessageInAllChat", msg);
            }

            await SendMessage(message, TypeOfMessage.Server, ChatDestination.All, string.Empty, string.Empty);
            var userDto = _mapper.Map<UserDto>(user);
            await Clients.Client(Context.ConnectionId).SendAsync("UpdateCurrentUser", userDto);
            await GetAllOnlineUsers();
        }

        public async Task DrawCard(string gameId)
        {
            var user = GetCurrentUser();
            var game = _gameRepository.GetGameByGameId(gameId);
            if (game.GameEnded || game.PlayerToPlay.User.Name != user.Name)
            {
                return;
            }

            if (game.PlayerToPlay.CardPromisedToDiscard != null)
            {
                _gameManager.DrawCard(game, game.PlayerToPlay, 2, false);
                game.PlayerToPlay.CardPromisedToDiscard = null;
                await AddToGameLog(gameId, $"Player didn't fulfill their promise, they will draw 2 cards. ");
            }

            _gameManager.DrawCard(game, game.PlayerToPlay, 1, true);
            await AddToGameLog(gameId, $"{user.Name} drew a card (normal draw)");
            await UpdateGame(game);
            await UpdateHands(game);
        }

        public async Task CheckUnoCall(string gameId, bool unoCalled)
        {
            if (!_userRepository.UserExistsByConnectionId(Context.ConnectionId))
            {
                return;
            }
            var user = GetCurrentUser();
            var game = _gameRepository.GetGameByGameId(gameId);
            var player = game.Players.First(x => x.User == user);

            if (!player.MustCallUno)
            {
                return;
            }

            if (unoCalled)
            {
                await SendMessage("*UNO!", ChatDestination.Game, gameId, string.Empty);
            }
            else
            {
                _gameManager.DrawCard(game, player, 2, false);
                await SendMessage($"Player [{player.User.Name}] forgot to call uno! They will draw 2 cards.", TypeOfMessage.Server, ChatDestination.Game, gameId, string.Empty);
                await UpdateGame(game);
                await UpdateHands(game);
            }
            player.MustCallUno = false;
        }

        public async Task PlayCard(string gameId, string cardPlayedId, CardColor targetedCardColor, string playerTargetedId, string cardToDigId, List<int> duelNumbers, List<string> charityCardsIds, int blackjackNumber, List<int> numbersToDiscard, string cardPromisedToDiscardId, string oddOrEvenGuess)
        {

            var game = _gameRepository.GetGameByGameId(gameId);
            if (game.GameEnded || !game.GameStarted)
                return;
            var user = GetCurrentUser();
            var player = game.Players.First(x => x.User.Name == user.Name);
            var moveResult = _playCardManager.PlayCard(game, player, cardPlayedId, targetedCardColor, playerTargetedId, cardToDigId, duelNumbers, charityCardsIds, blackjackNumber, numbersToDiscard, cardPromisedToDiscardId, oddOrEvenGuess);
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
                player.MustCallUno = true;
                await Clients.Caller.SendAsync("MustCallUno");
            }
            if (game.GameEnded)
            {
                int additionalPointsFromTournament = 0;
                if (game.IsTournamentGame)
                {
                    var tournament = _tournamentRepository.GetTournament(game.TournamentId);
                    _tournamentManager.UpdateTournament(tournament, game);
                    await UpdateTournament(tournament);
                    if (tournament.TournamentEnded)
                    {
                        additionalPointsFromTournament = (int)(game.GameSetup.RoundsToWin * tournament.Contestants.Count);
                    }
                }

                var pointsWon = game.GameSetup.PlayersSetup == PlayersSetup.Individual ? (int)(game.GameSetup.RoundsToWin * (Math.Pow(game.Players.Count, 2))) : (int)(game.GameSetup.RoundsToWin * (Math.Pow(game.Players.Select(x => x.TeamNumber).Distinct().Count(), 2)));
                pointsWon += additionalPointsFromTournament;
                var playersWon = game.Players.Where(x => x.RoundsWonCount == game.GameSetup.RoundsToWin).Select(x => x.User.Name).ToList();

                _hallOfFameRepository.AddPoints(playersWon, pointsWon);

                var hallOfFameStatsDto = _mapper.Map<List<HallOfFameDto>>(_hallOfFameRepository.GetScoresForUsernames(game.Players.Select(x => x.User.Name).ToList()));
                var gameEndedResultDto = new GameEndedResultDto(playersWon, pointsWon, hallOfFameStatsDto);

                await Clients.Clients(GetPlayersAndSpectatorsFromGame(game)).SendAsync("GameEnded", gameEndedResultDto);
            }
        }

        #region private

        private async Task AddToGameLog(string gameId, string message)
        {
            var game = _gameRepository.GetGameByGameId(gameId);
            game.GameLog.Add(message);
            var allUsersInGame = GetPlayersAndSpectatorsFromGame(game);
            await Clients.Clients(allUsersInGame).SendAsync("AddToGameLog", message);
        }

        private async Task DisplayToastMessageToGame(string gameid, string message)
        {
            var game = _gameRepository.GetGameByGameId(gameid);
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

        private async Task UpdateTournament(Tournament tournament)
        {
            var allUsersInTournament = GetPlayersAndSpectatorsFromTournament(tournament);
            var tournamentDto = _mapper.Map<TournamentDto>(tournament);
            await Clients.Clients(allUsersInTournament).SendAsync("UpdateTournament", tournamentDto);
        }
        private async Task UpdateHands(Game game)
        {
            if (game.GameStarted)
            {
                var allPlayersInTheGame = GetPlayersFromGame(game);
                foreach (var connectionId in allPlayersInTheGame)
                {
                    var myCards = game.Players.First(x => x.User.ConnectionId == connectionId).Cards;
                    var myCardsDto = _mapper.Map<List<CardDto>>(myCards).OrderBy(x => x.Color).ThenBy(x => x.Value);
                    await Clients.Client(connectionId).SendAsync("UpdateMyHand", myCardsDto);
                }
            }
        }

        private List<string> GetPlayersFromGame(Game game)
        {
            return game.Players.Where(x => !x.LeftGame).Select(y => y.User.ConnectionId).ToList();
        }
        private List<string> GetContestantsFromTournament(Tournament tournament)
        {
            return tournament.Contestants.Where(x => !x.LeftTournament).Select(y => y.User.ConnectionId).ToList();
        }
        private List<string> GetPlayersAndSpectatorsFromGame(Game game)
        {
            return GetPlayersFromGame(game).Concat(game.Spectators.Select(x => x.User.ConnectionId)).ToList();
        }

        private List<string> GetPlayersAndSpectatorsFromTournament(Tournament tournament)
        {
            return tournament.Contestants.Where(x => !x.LeftTournament).Select(y => y.User.ConnectionId).ToList().Concat(tournament.Spectators.Select(x => x.ConnectionId)).ToList();
        }

        private async Task CleanupUserFromGames(User user)
        {
            List<Game> games = _gameRepository.GetAllGames().Where(x => GetPlayersAndSpectatorsFromGame(x).Any(y => y == user.ConnectionId)).ToList();
            foreach (var game in games)
            {
                await ExitGame(game.Id, user);
            }
        }

        private async Task CleanupUserFromTournaments(User user)
        {
            List<Tournament> tournaments = _tournamentRepository.GetAllTournaments().Where(x => GetPlayersAndSpectatorsFromTournament(x).Any(y => y == user.ConnectionId)).ToList();
            foreach (var tournament in tournaments)
            {
                await ExitTournament(tournament.Id, user);
            }
        }

        private async Task CleanupUserFromGamesExceptThisGame(string gameId)
        {
            List<Game> games = _gameRepository.GetAllGames().Where(x => x.Id != gameId && GetPlayersAndSpectatorsFromGame(x).Any(y => y == Context.ConnectionId)).ToList();
            foreach (var game in games)
            {
                await ExitGame(game.Id);
            }
        }

        private async Task CleanupUserFromOnlineUsersList(User user)
        {
            _userRepository.RemoveUser(user);
            await GetAllOnlineUsers();
        }

        private async Task SendMessage(string message, TypeOfMessage typeOfMessage, ChatDestination chatDestination, string gameId, string tournamentId)
        {
            var user = GetCurrentUser();
            var username = typeOfMessage == TypeOfMessage.Server ? "Server" : user.Name;
            var chatMessageIntentionResult = GetChatMessageIntention(message);
            ChatMessageDto msgDto;
            bool buzzFailed = false;
            var allUsersInGame = new List<string>();
            var allUsersInTournament = new List<string>();
            Game game = null;
            Tournament tournament = null;
            if (!string.IsNullOrWhiteSpace(gameId))
            {
                game = _gameRepository.GetGameByGameId(gameId);
                allUsersInGame = GetPlayersAndSpectatorsFromGame(game);
            }
            if (!string.IsNullOrEmpty(tournamentId))
            {
                tournament = _tournamentRepository.GetTournament(tournamentId);
                allUsersInTournament = GetPlayersAndSpectatorsFromTournament(tournament);
            }
            if (chatMessageIntentionResult.ChatMessageIntention == ChatMessageIntention.Buzz)
            {
                var targetedUser = _userRepository.GetAllUsers().FirstOrDefault(x => x.Name == chatMessageIntentionResult.TargetedUsername);
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
                    await Clients.Caller.SendAsync("PostNewMessageInTournamentChat", msgDto);
                    await Clients.Caller.SendAsync("PostNewMessageInAllChat", msgDto);
                }
                else
                {
                    await Clients.Clients(allUsersInGame).SendAsync("PostNewMessageInGameChat", msgDto);
                    await Clients.Clients(allUsersInTournament).SendAsync("PostNewMessageInTournamentChat", msgDto);
                    await Clients.All.SendAsync("PostNewMessageInAllChat", msgDto);
                }

            }
            else if (chatMessageIntentionResult.ChatMessageIntention == ChatMessageIntention.Normal)
            {
                var msg = new ChatMessage(username, message, typeOfMessage);
                msgDto = _mapper.Map<ChatMessageDto>(msg);
                if (chatDestination == ChatDestination.Game)
                {
                    game.ChatMessages.Add(msg);
                    await Clients.Clients(allUsersInGame).SendAsync("PostNewMessageInGameChat", msgDto);
                }
                else if (chatDestination == ChatDestination.Tournament)
                {
                    tournament.ChatMessages.Add(msg);
                    await Clients.Clients(allUsersInTournament).SendAsync("PostNewMessageInTournamentChat", msgDto);
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

                    if (_userRepository.UserExistsByName(username))
                    {
                        var user = _userRepository.GetUserByName(username);
                        if (user != null && !mentionedUsers.Contains(user))
                        {
                            mentionedUsers.Add(user);
                        }
                    }

                });

                return new ChatMessageIntentionResult() { ChatMessageIntention = ChatMessageIntention.Normal, MentionedUsers = mentionedUsers };
            }
        }

        private async Task ExitTournament(string tournamentId, User user)
        {
            var tournament = _tournamentRepository.GetTournament(tournamentId);

            await SendMessage($"{user.Name} has left the tournament.", TypeOfMessage.Server, ChatDestination.Tournament, string.Empty, tournamentId);

            var contestant = tournament.Contestants.FirstOrDefault(x => x.User == user);

            if (contestant != null)
            {
                if (tournament.TournamentStarted)
                {
                    contestant.LeftTournament = true;
                }
                else
                {
                    tournament.Contestants.Remove(contestant);
                }
            }

            var spectator = tournament.Spectators.FirstOrDefault(x => x == user);
            if (spectator != null)
                tournament.Spectators.Remove(spectator);


            await Clients.Caller.SendAsync("ExitTournament");
            await UpdateTournament(tournament);

            if (!GetPlayersAndSpectatorsFromTournament(tournament).Any())
            {
                tournament.TournamentRounds.ForEach(x =>
                {
                    x.TournamentRoundGames.ForEach(y => { _gameRepository.RemoveGame(y.Game); });
                });
                _tournamentRepository.RemoveTournament(tournament);
            }

            await GetAllTournaments();
        }

        private async Task ExitGame(string gameId, User user)
        {
            var game = _gameRepository.GetGameByGameId(gameId);
            var allPlayersFromGame = GetPlayersFromGame(game);
            if (allPlayersFromGame.Contains(user.ConnectionId))
            {
                var player = game.Players.First(y => y.User.ConnectionId == user.ConnectionId);
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
                game.Spectators.Remove(game.Spectators.First(x => x.User.ConnectionId == user.ConnectionId));
            }
            await UpdateGame(game);
            await SendMessage($"{user.Name} has left the game.", TypeOfMessage.Server, ChatDestination.Game, gameId, string.Empty);
            if (game.Players.All(x => x.LeftGame) && !game.Spectators.Any() && !game.IsTournamentGame)
            {
                _gameRepository.RemoveGame(game);
            }
            await GetAllGames();
            await Clients.Caller.SendAsync("ExitGame");
        }

        private User GetCurrentUser()
        {
            return _userRepository.GetUserByConnectionId(Context.ConnectionId);
        }


        #endregion

    }
}