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
using GameProcessingService.CardEffectProcessors.AutomaticallyTriggered;
using GameProcessingService.CoreManagers;
using GameProcessingService.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using PreMoveProcessingService.CoreManagers;
using Repository;
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
        private readonly IEnumerable<IAutomaticallyTriggeredCardEffectProcessor> _automaticallyTriggeredCardEffectProcessors;
        private readonly AppSettings _appSettings;

        public GameHub(IMapper mapper, IGameManager gameManager, IPlayCardManager playCardManager, IUserRepository userRepository, IGameRepository gameRepository, IHallOfFameRepository hallOfFameRepository, ITournamentRepository tournamentRepository, ITournamentManager tournamentManager, IOptions<AppSettings> appSettings, IEnumerable<IAutomaticallyTriggeredCardEffectProcessor> automaticallyTriggeredCardEffectProcessors)
        {
            _gameManager = gameManager;
            _playCardManager = playCardManager;
            _userRepository = userRepository;
            _gameRepository = gameRepository;
            _hallOfFameRepository = hallOfFameRepository;
            _tournamentRepository = tournamentRepository;
            _tournamentManager = tournamentManager;
            _automaticallyTriggeredCardEffectProcessors = automaticallyTriggeredCardEffectProcessors;
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
                await SendMessage($"{user.Name} has left the server.", TypeOfMessage.Server, ChatDestination.All, user);
                await ExitGame(user);
                await ExitTournament(user);
                await CleanupUserFromOnlineUsersList(user);
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string message, ChatDestination chatDestination)
        {
            var user = GetCurrentUser();
            var isPlayer = true;

            if (chatDestination == ChatDestination.Game)
            {
                var game = _gameRepository.GetGameByGameId(user.ActiveGameId);
                isPlayer = GetPlayersFromGame(game).FirstOrDefault(x => x == Context.ConnectionId) != null;
            }

            else if (chatDestination == ChatDestination.Tournament)
            {
                var tournament = _tournamentRepository.GetTournament(user.ActiveTournamentId);
                isPlayer = GetContestantsFromTournament(tournament).FirstOrDefault(x => x == Context.ConnectionId) != null;
            }

            await SendMessage(message, isPlayer ? TypeOfMessage.Chat : TypeOfMessage.Spectators, chatDestination, user);
        }

        public async Task GetAllOnlineUsers()
        {
            var usersDto = _mapper.Map<List<UserDto>>(_userRepository.GetAllUsers());
            await Clients.All.SendAsync("RefreshOnlineUsersList", usersDto.OrderBy(x => x.Name));
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

            var bannedUser = tournament.BannedUsers.FirstOrDefault(x => x.Name == user.Name);
            if (bannedUser != null)
            {
                await DisplayToastMessageToUser(user.ConnectionId, "You have been banned from this tournament.", "error");
                return;
            }

            var spectator = tournament.Spectators.FirstOrDefault(x => x.Name == user.Name);
            if (!string.IsNullOrEmpty(tournament.TournamentSetup.Password) && spectator == null)
                if (tournament.TournamentSetup.Password != password)
                {
                    await DisplayToastMessageToUser(user.ConnectionId, "Incorrect password.", "error");
                    return;
                }

            user.ActiveTournamentId = tournament.Id;

            if (!tournament.TournamentStarted)
            {
                if (spectator != null)
                {
                    if (tournament.ReadyPhaseExpireUtc > DateTime.Now)
                    {
                        await DisplayToastMessageToUser(Context.ConnectionId, "Tournament is in the ready phase. You can't join at this time.", "info");
                        return;
                    }

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
                    await SendMessage($"{user.Name} has joined the tournament.", TypeOfMessage.Server, ChatDestination.Tournament, user);
                }
            }
            else
            {
                var contestant = tournament.Contestants.FirstOrDefault(x => x.User.Name == user.Name);
                if (contestant != null)
                {
                    contestant.User = user;
                    contestant.LeftTournament = false;
                }
                else if (spectator == null)
                {
                    tournament.Spectators.Add((user));
                }
                await SendMessage($"{user.Name} has joined the tournament.", TypeOfMessage.Server, ChatDestination.Tournament, user);
            }

            var chatMsgs = tournament.ChatMessages.ToList();
            chatMsgs.Reverse();
            await Clients.Caller.SendAsync("RetrieveFullChat", _mapper.Map<List<ChatMessageDto>>(chatMsgs), ChatDestination.Tournament);
            await UpdateTournament(tournament);
            await GetAllTournaments();
        }

        public async Task CreateTournament(TournamentSetupDto tournamentSetupDto, string password)
        {
            if (string.IsNullOrEmpty(password) || !string.Equals(password, _appSettings.AdminPassword))
            {
                await DisplayToastMessageToUser(Context.ConnectionId, "Unauthorized", "error");
                return;
            }

            if (string.IsNullOrEmpty(tournamentSetupDto.Name))
            {
                tournamentSetupDto.Name = "tour.";
            }

            else if (tournamentSetupDto.Name.Length > 25)
            {
                tournamentSetupDto.Name = tournamentSetupDto.Name.Substring(0, 25);
            }
            var tournamentSetup = _mapper.Map<TournamentSetup>(tournamentSetupDto);
            var tournament = new Tournament(tournamentSetup);
            tournament.Contestants.Add(new Contestant(GetCurrentUser()));
            _tournamentRepository.AddTournament(tournament);

            var user = GetCurrentUser();
            user.ActiveTournamentId = tournament.Id;

            await SendMessage($"User {user.Name} has created new tournament", TypeOfMessage.Server, ChatDestination.All, user);
            await UpdateTournament(tournament);
            await GetAllTournaments();
        }

        public async Task StartTournament()
        {
            var user = GetCurrentUser();
            var tournament = _tournamentRepository.GetTournament(user.ActiveTournamentId);

            if (tournament.TournamentStarted || tournament.Contestants.Count < 3 || tournament.Contestants.First().User != user)
                return;

            if (tournament.ReadyPhaseExpireUtc > DateTime.Now)
            {
                await DisplayToastMessageToUser(Context.ConnectionId, "Tournament is in the ready phase. You can't start the tournament now.", "info");
                return;
            }

            tournament.ReadyPhaseExpireUtc = DateTime.Now.AddSeconds(10);
            tournament.ReadyPlayersLeft = tournament.Contestants.Select(x => x.User.Name).ToList();
            await UpdateTournament(tournament);
            await Clients.Clients(GetContestantsFromTournament(tournament)).SendAsync("StartModalPhasePlayers", true);
            await Clients.Clients(GetSpectatorsFromTournament(tournament)).SendAsync("StartModalPhaseSpectators", true);
        }


        public async Task ReadyForTournament()
        {
            var user = GetCurrentUser();
            var tournament = _tournamentRepository.GetTournament(user.ActiveTournamentId);
            var player = tournament.Contestants.FirstOrDefault(x => x.User.Name == user.Name);
            if (tournament.TournamentStarted || player == null || DateTime.Now > tournament.ReadyPhaseExpireUtc || !tournament.ReadyPlayersLeft.Contains(user.Name))
            {
                return;
            }

            tournament.ReadyPlayersLeft.Remove(user.Name);
            await UpdateTournament(tournament);
            if (!tournament.ReadyPlayersLeft.Any())
            {
                _tournamentManager.StartTournament(tournament);
                await UpdateTournament(tournament);
                await GetAllTournaments();
                await Clients.Clients(GetContestantsAndSpectatorsFromTournament(tournament)).SendAsync("TournamentStarted");
            }
        }

        public async Task ExitTournament()
        {
            var user = GetCurrentUser();
            await ExitTournament(user);
        }

        public async Task CreateGame(GameSetupDto gameSetupDto)
        {
            var user = GetCurrentUser();
            var gameSetup = _mapper.Map<GameSetup>(gameSetupDto);
            var game = new Game(gameSetup);
            game.Players.Add(new Player(user, game.Players.Count + 1));
            _gameRepository.AddGame(game);

            user.ActiveGameId = game.Id;

            await UpdateGame(game);
            await GetAllGames();
            await SendMessage($"User {user.Name} has created new game", TypeOfMessage.Server, ChatDestination.All, user);
        }

        public async Task AdminBuzzAll(string password)
        {
            if (string.IsNullOrEmpty(password) || !string.Equals(password, _appSettings.AdminPassword))
            {
                await DisplayToastMessageToUser(Context.ConnectionId, "Unauthorized", "error");
                return;
            }

            var allPlayers = _userRepository.GetAllUsers();

            var msgDto = _mapper.Map<ChatMessageDto>(new ChatMessage("Server", $"Moderator has dinged all the users!", TypeOfMessage.Server));


            foreach (var player in allPlayers)
            {
                await Clients.Client(player.ConnectionId).SendAsync("BuzzPlayer", "ding");
                if (!string.IsNullOrEmpty(player.ActiveGameId))
                {
                    await Clients.Client(player.ConnectionId).SendAsync("PostNewMessage", msgDto, ChatDestination.Game);
                }
                if (!string.IsNullOrEmpty(player.ActiveTournamentId))
                {
                    await Clients.Client(player.ConnectionId).SendAsync("PostNewMessage", msgDto, ChatDestination.Tournament);
                }
                await Clients.Client(player.ConnectionId).SendAsync("PostNewMessage", msgDto, ChatDestination.All);
            }

        }
        public async Task AdminKickUser(string name, string password)
        {
            if (string.IsNullOrEmpty(password) || !string.Equals(password, _appSettings.AdminPassword))
            {
                await DisplayToastMessageToUser(Context.ConnectionId, "Unauthorized", "error");
                return;
            }

            if (_userRepository.UserExistsByName(name))
            {
                var user = _userRepository.GetUserByName(name);
                await Clients.Client(user.ConnectionId).SendAsync("AdminKickUser");
                await SendMessage($"{user.Name} has left the server.", TypeOfMessage.Server, ChatDestination.All, user);
                await ExitGame(user);
                await ExitTournament(user);
                await CleanupUserFromOnlineUsersList(user);
            }
        }


        public async Task AdminCleanupGame(string gameId, string password)
        {
            if (string.IsNullOrEmpty(password) || !string.Equals(password, _appSettings.AdminPassword))
            {
                await DisplayToastMessageToUser(Context.ConnectionId, "Unauthorized", "error");
                return;
            }

            var game = _gameRepository.GetGameByGameId(gameId);

            var host = game.Players.First().User.Name;

            var playersAndSpecs = GetPlayersAndSpectatorsFromGame(game);
            playersAndSpecs.ForEach(async x =>
            {
                if (_userRepository.UserExistsByConnectionId(x))
                {
                    var user = _userRepository.GetUserByConnectionId(x);
                    await Clients.Client(user.ConnectionId).SendAsync("SendToTheLobby");
                    await ExitGame(user);

                }
            });
            _gameRepository.RemoveGame(game);
            await GetAllGames();
            await SendMessage($"Moderator has cleaned up the game [{host}]", TypeOfMessage.Server, ChatDestination.All, GetCurrentUser());
        }

        public async Task AdminCleanupTournament(string tournamentId, string password)
        {
            if (string.IsNullOrEmpty(password) || !string.Equals(password, _appSettings.AdminPassword))
            {
                await DisplayToastMessageToUser(Context.ConnectionId, "Unauthorized", "error");
                return;
            }

            var tournament = _tournamentRepository.GetTournament(tournamentId);
            var playersAndSpecsTournament = GetContestantsAndSpectatorsFromTournament(tournament);

            var allGamesWithinTournament = _gameRepository.GetAllGames().Where(x => x.TournamentId == tournamentId).ToList();

            allGamesWithinTournament.ForEach(x =>
            {
                var playersAndSpecsGame = GetPlayersAndSpectatorsFromGame(x);
                playersAndSpecsGame.ForEach(async x =>
                {
                    if (_userRepository.UserExistsByConnectionId(x))
                    {
                        var user = _userRepository.GetUserByConnectionId(x);
                        await Clients.Client(user.ConnectionId).SendAsync("SendToTheTournament");
                        await ExitGame(user);
                    }
                });
                _gameRepository.RemoveGame(x);
            });

            playersAndSpecsTournament.ForEach(async x =>
            {
                if (_userRepository.UserExistsByConnectionId(x))
                {
                    var user = _userRepository.GetUserByConnectionId(x);
                    await Clients.Client(user.ConnectionId).SendAsync("SendToTheLobby");
                    await ExitTournament(user);
                }
            });
            _tournamentRepository.RemoveTournament(tournament);
            await GetAllGames();
            await GetAllTournaments();
            await SendMessage($"Moderator has cleaned up the tournament [{tournament.TournamentSetup.Name}]", TypeOfMessage.Server, ChatDestination.All, GetCurrentUser());
        }

        public async Task ExitGame()
        {
            var user = GetCurrentUser();
            await ExitGame(user);
        }

        public async Task ChangeTeam(int teamNumber)
        {
            var user = GetCurrentUser();
            if (teamNumber < 1 || teamNumber > 5)
                return;

            var game = _gameRepository.GetGameByGameId(user.ActiveGameId);
            if (game.GameSetup.PlayersSetup != PlayersSetup.Teams)
                return;
            var player = game.Players.First(x => x.User == user);
            player.TeamNumber = teamNumber;
            await UpdateGame(game);
        }

        public async Task KickPlayerFromGame(bool isBan, string name)
        {
            var user = GetCurrentUser();
            var game = _gameRepository.GetGameByGameId(user.ActiveGameId);
            if (!Context.ConnectionId.Equals(game.Players.First().User.ConnectionId) || game.GameStarted)
            {
                return;
            }

            var isPlayer = game.Players.FirstOrDefault(x => x.User.Name == name);

            //check if spectator or a player
            var userToKick = isPlayer != null ? isPlayer.User : game.Spectators.First(x => x.User.Name == name).User;

            if (isBan)
            {
                game.BannedUsers.Add(userToKick);
            }

            var action = isBan ? "banned" : "kicked";

            await DisplayToastMessageToUser(userToKick.ConnectionId, $"You have been {action} from the game.", "error");
            await Clients.Client(userToKick.ConnectionId).SendAsync("SendToTheLobby");
            await ExitGame(userToKick);
            await SendMessage($"Player {userToKick.Name} was {action} from the game.", TypeOfMessage.Server, ChatDestination.Game, user);
        }

        public async Task UnbanPlayerFromGame(string name)
        {
            var user = GetCurrentUser();
            var game = _gameRepository.GetGameByGameId(user.ActiveGameId);
            if (!Context.ConnectionId.Equals(game.Players.First().User.ConnectionId) || game.GameStarted)
            {
                return;
            }
            var userToUnban = game.BannedUsers.First(y => y.Name == name);
            game.BannedUsers.Remove(userToUnban);
            await SendMessage($"Player {userToUnban.Name} was unbanned from the game.", TypeOfMessage.Server, ChatDestination.Game, user);
            await UpdateGame(game);
        }



        public async Task KickContestantFromTournament(bool isBan, string name)
        {
            var user = GetCurrentUser();
            var tournament = _tournamentRepository.GetTournament(user.ActiveTournamentId);
            if (!Context.ConnectionId.Equals(tournament.Contestants.First().User.ConnectionId) || tournament.TournamentStarted)
            {
                return;
            }

            var isPlayer = tournament.Contestants.FirstOrDefault(x => x.User.Name == name);

            //check if spectator or a player
            var userToKick = isPlayer != null ? isPlayer.User : tournament.Spectators.First(x => x.Name == name);

            if (isBan)
            {
                tournament.BannedUsers.Add(userToKick);
            }

            var action = isBan ? "banned" : "kicked";

            await DisplayToastMessageToUser(userToKick.ConnectionId, $"You have been {action} from the tournament.", "error");
            await Clients.Client(userToKick.ConnectionId).SendAsync("SendToTheLobby");
            await ExitTournament(userToKick);
            await SendMessage($"Player {userToKick.Name} was {action} from the tournament.", TypeOfMessage.Server, ChatDestination.Tournament, user);
        }

        public async Task UnbanContestantFromTournament(string name)
        {
            var user = GetCurrentUser();
            var tournament = _tournamentRepository.GetTournament(user.ActiveTournamentId);
            if (!Context.ConnectionId.Equals(tournament.Contestants.First().User.ConnectionId) || tournament.TournamentStarted)
            {
                return;
            }
            var userToUnban = tournament.BannedUsers.First(y => y.Name == name);
            tournament.BannedUsers.Remove(userToUnban);
            await SendMessage($"Player {userToUnban.Name} was unbanned from the tournament.", TypeOfMessage.Server, ChatDestination.Tournament, user);
            await UpdateTournament(tournament);
        }

        public async Task UpdateGameSetup(GameSetupDto gameSetupDto)
        {
            var user = GetCurrentUser();
            var game = _gameRepository.GetGameByGameId(user.ActiveGameId);
            if (!Context.ConnectionId.Equals(game.Players.First().User.ConnectionId) || game.GameStarted)
            {
                return;
            }

            var gameSetup = _mapper.Map<GameSetup>(gameSetupDto);
            game.GameSetup = gameSetup;
            await UpdateGame(game);
            await GetAllGames();
        }
        public async Task UpdateTournamentSetup(TournamentSetupDto tournamentSetupDto)
        {
            var user = GetCurrentUser();
            var tournament = _tournamentRepository.GetTournament(user.ActiveTournamentId);
            if (!Context.ConnectionId.Equals(tournament.Contestants.First().User.ConnectionId) || tournament.TournamentStarted)
            {
                return;
            }

            var tournamentSetup = _mapper.Map<TournamentSetup>(tournamentSetupDto);

            if (string.IsNullOrEmpty(tournamentSetup.Name))
            {
                tournamentSetup.Name = "unnamed";
            }

            else if (tournamentSetup.Name.Length > 25)
            {
                tournamentSetup.Name = tournamentSetup.Name.Substring(0, 25);

            }

            tournament.TournamentSetup = tournamentSetup;
            await UpdateTournament(tournament);
            await GetAllTournaments();
        }

        public async Task StartGame()
        {
            var user = GetCurrentUser();
            var game = _gameRepository.GetGameByGameId(user.ActiveGameId);

            if (!Context.ConnectionId.Equals(game.Players.First().User.ConnectionId) || game.GameStarted)
            {
                return;
            }

            if (DateTime.Now <= game.ReadyPhaseExpireUtc)
            {
                await DisplayToastMessageToUser(Context.ConnectionId, "Game is in the ready phase. You can't start the game again", "info");
                return;
            }

            game.ReadyPhaseExpireUtc = DateTime.Now.AddSeconds(10);
            game.ReadyPlayersLeft = game.Players.Select(x => x.User.Name).ToList();

            await UpdateGame(game);
            await Clients.Clients(GetPlayersFromGame(game)).SendAsync("StartModalPhasePlayers", false);
            await Clients.Clients(GetSpectatorsFromGame(game)).SendAsync("StartModalPhaseSpectators", false);
        }

        public async Task ReadyForGame()
        {
            var user = GetCurrentUser();
            var gameId = user.ActiveGameId;
            var game = _gameRepository.GetGameByGameId(gameId);
            var player = game.Players.FirstOrDefault(x => x.User.Name == user.Name);
            if (game.GameStarted || player == null || DateTime.Now > game.ReadyPhaseExpireUtc || !game.ReadyPlayersLeft.Contains(user.Name))
            {
                return;
            }

            game.ReadyPlayersLeft.Remove(user.Name);
            await UpdateGame(game);
            if (!game.ReadyPlayersLeft.Any())
            {
                _gameManager.StartNewGame(game);
                await UpdateGame(game);
                await UpdateHands(game);
                await GetAllGames();
                await Clients.Clients(GetPlayersAndSpectatorsFromGame(game)).SendAsync("GameStarted");
                await AddToGameLog(gameId, "Game started! (type /hand in chat for newbie tips)");
                await AddToGameLog(gameId, "If you need more detailed log info, press the 'Game info' button.");
                await AddToGameLog(gameId, "This is the game log summary. We will display the last 3 entries here.");
            }
        }

        public async Task JoinGame(string gameId, string password)
        {
            var user = GetCurrentUser();
            var game = _gameRepository.GetGameByGameId(gameId);

            if (game.IsTournamentGame && !game.GameStarted)
                return;

            var bannedUser = game.BannedUsers.FirstOrDefault(x => x.Name == user.Name);
            if (bannedUser != null)
            {
                await DisplayToastMessageToUser(user.ConnectionId, "You have been banned from this game.", "error");
                return;
            }

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
                    await DisplayToastMessageToUser(user.ConnectionId, "Incorrect password.", "error");
                    return;
                }

            var spectator = game.Spectators.FirstOrDefault(x => x.User.Name == user.Name);

            user.ActiveGameId = game.Id;

            if (!game.GameStarted)
            {
                if (spectator != null)
                {
                    //join the game that hasn't started
                    if (game.ReadyPhaseExpireUtc > DateTime.Now)
                    {
                        await DisplayToastMessageToUser(Context.ConnectionId, "Game is in the ready phase. You can't join at this time", "info");
                        return;
                    }

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
                    await SendMessage($"{user.Name} has joined the game room.", TypeOfMessage.Server, ChatDestination.Game, user);
                }
            }
            else
            {
                var playerLeftWithThisName = game.Players.FirstOrDefault(x => x.User.Name == user.Name);
                if (playerLeftWithThisName != null)
                {
                    playerLeftWithThisName.User = user;
                    playerLeftWithThisName.LeftGame = false;
                    await DisplayToastMessageToGame(gameId, $"Player {user.Name} has reconnected to the game.", "info");
                }
                else if (spectator == null)
                {
                    game.Spectators.Add(new Spectator(user));
                }
                await SendMessage($"{user.Name} has joined the game room.", TypeOfMessage.Server, ChatDestination.Game, user);
            }
            var chatMsgs = game.ChatMessages.ToList();
            chatMsgs.Reverse();
            var logs = game.GameLog.ToList();
            logs.Reverse();

            await Clients.Caller.SendAsync("RetrieveFullChat", _mapper.Map<List<ChatMessageDto>>(chatMsgs), ChatDestination.Game);
            await Clients.Caller.SendAsync("RetrieveFullGameLog", logs);
            await UpdateHands(game);
            await UpdateGame(game);
            await GetAllGames();
        }

        public async Task AddOrRenameUser(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                await Clients.Caller.SendAsync("RenamePlayer");
                return;
            };

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
                if (!string.IsNullOrEmpty(user.ActiveGameId))
                {
                    var game = _gameRepository.GetGameByGameId(user.ActiveGameId);
                    var bannedUser = game.BannedUsers.FirstOrDefault(x => x.Name == user.Name);
                    if (bannedUser != null)
                    {
                        game.BannedUsers.Remove(bannedUser);
                    }
                }
                if (!string.IsNullOrEmpty(user.ActiveTournamentId))
                {
                    var tournament = _tournamentRepository.GetTournament(user.ActiveTournamentId);
                    var bannedUser = tournament.BannedUsers.FirstOrDefault(x => x.Name == user.Name);
                    if (bannedUser != null)
                    {
                        tournament.BannedUsers.Remove(bannedUser);
                    }
                }
            }
            else
            {
                message = $"{name} has connected to the server.";
                user = new User(Context.ConnectionId, name);
                _userRepository.AddUser(user);
                ChatMessageDto msg = new ChatMessageDto() { CreatedUtc = DateTime.Now, Text = "If you need to create a tournament please contact one of the moderators (discord link in the navbar)", TypeOfMessage = TypeOfMessage.Server, Username = "Server" };
                await Clients.Caller.SendAsync("PostNewMessage", msg, ChatDestination.All);
            }

            await SendMessage(message, TypeOfMessage.Server, ChatDestination.All, user);
            var userDto = _mapper.Map<UserDto>(user);
            await Clients.Client(Context.ConnectionId).SendAsync("UpdateCurrentUser", userDto);
            await GetAllOnlineUsers();
        }

        public async Task DrawCard()
        {
            var user = GetCurrentUser();
            var gameId = user.ActiveGameId;
            var game = _gameRepository.GetGameByGameId(gameId);
            if (game.GameEnded || game.PlayerToPlay.User.Name != user.Name)
            {
                return;
            }

            if (game.SilenceTurnsRemaining <= 0 && game.PlayerToPlay.Cards.Count > 4 && game.PlayerToPlay.Cards.FirstOrDefault(x => x.Value == CardValue.KingsDecree) != null)
            {
                await AddToGameLog(game.Id, $"{game.PlayerToPlay.User.Name} is not affected by the draw. He has more than 4 cards and king's decree in hand (auto effect is activated).");
                game.PlayerToPlay.CardPromisedToDiscard = null;
                var nextPlayer = _gameManager.GetNextPlayer(game, game.PlayerToPlay, game.Players, false);
                game.PlayerToPlay = nextPlayer;

                var automaticallyTriggeredResultQueensDecree = _automaticallyTriggeredCardEffectProcessors.First(x => x.CardAffected == CardValue.QueensDecree).ProcessCardEffect(game, string.Empty, new AutomaticallyTriggeredParams() { QueensDecreeParams = new AutomaticallyTriggeredQueensDecreeParams() { PlayerAffected = game.PlayerToPlay } });
                if (!string.IsNullOrEmpty(automaticallyTriggeredResultQueensDecree.MessageToLog))
                    await AddToGameLog(gameId, automaticallyTriggeredResultQueensDecree.MessageToLog);

                await UpdateGame(game);
                await UpdateHands(game);
                return;
            }


            if (game.PlayerToPlay.CardPromisedToDiscard != null)
            {
                _gameManager.DrawCard(game, game.PlayerToPlay, 2, false);
                game.PlayerToPlay.CardPromisedToDiscard = null;
                await AddToGameLog(gameId, $"Player didn't fulfill their promise, they will draw 2 cards. ");
            }

            var cardToDraw = game.Deck.Cards.Take(1).First();

            if (game.GameSetup.DrawAutoPlay && (cardToDraw.Color == game.LastCardPlayed.Color || (cardToDraw.Value == game.LastCardPlayed.Value && !game.LastCardPlayed.WasWildCard)))
            {
                _gameManager.DrawCard(game, game.PlayerToPlay, 1, false);
                await AddToGameLog(gameId, $"{user.Name} drew and autoplayed a card.");
                await PlayCard(cardToDraw.Id, cardToDraw.Color, string.Empty, string.Empty, null, null, 0, null, string.Empty, string.Empty, null, false);
                      return;
            }

            _gameManager.DrawCard(game, game.PlayerToPlay, 1, true);
            await AddToGameLog(gameId, $"{user.Name} drew a card (normal draw)");

            var automaticallyTriggeredResultQueensDecree2 = _automaticallyTriggeredCardEffectProcessors.First(x => x.CardAffected == CardValue.QueensDecree).ProcessCardEffect(game, string.Empty, new AutomaticallyTriggeredParams() { QueensDecreeParams = new AutomaticallyTriggeredQueensDecreeParams() { PlayerAffected = game.PlayerToPlay } });
            if (!string.IsNullOrEmpty(automaticallyTriggeredResultQueensDecree2.MessageToLog))
                await AddToGameLog(gameId, automaticallyTriggeredResultQueensDecree2.MessageToLog);

            await UpdateGame(game);
            await UpdateHands(game);

            if (game.HandCuffedPlayers.Contains(game.PlayerToPlay))
            {
                var nextPlayerToPlay = _gameManager.GetNextPlayer(game, game.PlayerToPlay, game.Players);
                var messageToLog = $"{game.PlayerToPlay.User.Name} was handcuffed so he will skip this turn. Player to play: {nextPlayerToPlay.User.Name}";
                game.HandCuffedPlayers.Remove(game.PlayerToPlay);
                game.PlayerToPlay = nextPlayerToPlay;
                await AddToGameLog(game.Id, messageToLog);
            }
            if (game.SilenceTurnsRemaining > 0)
            {
                game.SilenceTurnsRemaining--;
                var messageToLog = $"{game.SilenceTurnsRemaining} silenced turns remaining. ";
                await AddToGameLog(game.Id, messageToLog);
            }
        }

        public async Task CheckUnoCall(bool unoCalled)
        {
            var user = GetCurrentUser();
            var gameId = user.ActiveGameId;
            var game = _gameRepository.GetGameByGameId(gameId);
            if (game == null || game.GameEnded)
                return;
            var player = game.Players.First(x => x.User == user);

            if (!player.MustCallUno)
            {
                return;
            }

            if (unoCalled)
            {
                await DisplayToastMessageToGame(gameId, $"{player.User.Name} called *UNO!", "warning");
            }
            else
            {
                _gameManager.DrawCard(game, player, 2, false);
                await SendMessage($"Player [{player.User.Name}] forgot to call uno! They will draw 2 cards.", TypeOfMessage.Server, ChatDestination.Game, user);
                await UpdateGame(game);
                await UpdateHands(game);
            }
            player.MustCallUno = false;
        }

        public async Task ShowTeammatesHand()
        {
            var user = GetCurrentUser();
            var game = _gameRepository.GetGameByGameId(user.ActiveGameId);
            if (game.GameSetup.PlayersSetup != PlayersSetup.Teams || !game.GameSetup.CanSeeTeammatesHandInTeamGame)
            {
                return;
            }
            var player = game.Players.First(x => x.User.Name == user.Name);

            List<KeyValuePair<string, List<CardDto>>> result = new List<KeyValuePair<string, List<CardDto>>>();
            game.Players.Where(x => x.TeamNumber == player.TeamNumber && x != player).ToList().ForEach(x =>
                  {
                      result.Add(new KeyValuePair<string, List<CardDto>>($"{x.User.Name}'s cards", _mapper.Map<List<CardDto>>(x.Cards).OrderBy(y => y.Color).ThenBy(y => y.Value).ToList()));
                  });

            await Clients.Caller.SendAsync(Constants.Commands.SHOW_CARDS_CALLBACK_COMMAND, result, true);

        }

        public async Task PlayCard(string cardPlayedId, CardColor targetedCardColor, string playerTargetedId, string cardToDigId, List<int> duelNumbers, List<string> charityCardsIds, int blackjackNumber, List<int> numbersToDiscard, string cardPromisedToDiscardId, string oddOrEvenGuess, CardValue? targetedCardValue, bool activateSpecialCardEffect)
        {
            var user = GetCurrentUser();
            var gameId = user.ActiveGameId;
            var game = _gameRepository.GetGameByGameId(gameId);
            if (game.GameEnded || !game.GameStarted)
                return;
            var player = game.Players.First(x => x.User.Name == user.Name);
            var moveResult = _playCardManager.PlayCard(game, player, cardPlayedId, targetedCardColor, playerTargetedId, cardToDigId, duelNumbers, charityCardsIds, blackjackNumber, numbersToDiscard, cardPromisedToDiscardId, oddOrEvenGuess, targetedCardValue, activateSpecialCardEffect);
            if (moveResult == null)
            {
                return;
            }
            moveResult.MoveResultCallbackParams.ForEach(async callbackParam =>
            {
                if (!game.GameEnded)
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

            if (game.RoundEnded)
            {
                game.RoundEnded = false;
                if (game.IsTournamentGame)
                {
                    var tournament = _tournamentRepository.GetTournament(game.TournamentId);
                    await UpdateTournament(tournament);
                }
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

        private async Task DisplayToastMessageToGame(string gameId, string message, string toastrType)
        {
            var game = _gameRepository.GetGameByGameId(gameId);
            var allUsersInGame = GetPlayersAndSpectatorsFromGame(game);
            await Clients.Clients(allUsersInGame).SendAsync("DisplayToastMessage", message, toastrType);
        }

        private async Task DisplayToastMessageToUser(string connectionId, string message, string toastrType)
        {
            await Clients.Client(connectionId).SendAsync("DisplayToastMessage", message, toastrType);
        }

        private async Task UpdateGame(Game game)
        {
            var gameDto = _mapper.Map<GameDto>(game);
            var allUsersInGame = GetPlayersAndSpectatorsFromGame(game);
            await Clients.Clients(allUsersInGame).SendAsync("UpdateGame", gameDto);
        }

        private async Task UpdateTournament(Tournament tournament)
        {
            var allUsersInTournament = GetContestantsAndSpectatorsFromTournament(tournament);
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
                await Clients.Client(game.PlayerToPlay.User.ConnectionId).SendAsync("BuzzMyTurnToPlay");
                if (game.GameSetup.SpectatorsCanViewHands)
                {
                    var allSpectatorsInGame = GetSpectatorsFromGame(game);
                    List<KeyValuePair<string, List<CardDto>>> spectatorsViewHandsAndUser = new List<KeyValuePair<string, List<CardDto>>>();
                    game.Players.ForEach(x =>
                    {
                        spectatorsViewHandsAndUser.Add(new KeyValuePair<string, List<CardDto>>(x.User.Name, _mapper.Map<List<CardDto>>(x.Cards).OrderBy(c => c.Color).ThenBy(c => c.Value).ToList()));
                    });
                    await Clients.Clients(allSpectatorsInGame).SendAsync("UpdateSpectatorsViewHandsAndUser", spectatorsViewHandsAndUser);
                }

            }
        }

        private List<string> GetPlayersFromGame(Game game)
        {
            return game.Players.Where(x => !x.LeftGame).Select(y => y.User.ConnectionId).ToList();
        }
        private List<string> GetSpectatorsFromGame(Game game)
        {
            return game.Spectators.Select(x => x.User.ConnectionId).ToList();
        }
        private List<string> GetPlayersAndSpectatorsFromGame(Game game)
        {
            return GetPlayersFromGame(game).Concat(GetSpectatorsFromGame(game)).ToList();
        }
        private List<string> GetContestantsFromTournament(Tournament tournament)
        {
            return tournament.Contestants.Where(x => !x.LeftTournament).Select(y => y.User.ConnectionId).ToList();
        }
        private List<string> GetSpectatorsFromTournament(Tournament tournament)
        {
            return tournament.Spectators.Select(y => y.ConnectionId).ToList();
        }

        private List<string> GetContestantsAndSpectatorsFromTournament(Tournament tournament)
        {
            return GetContestantsFromTournament(tournament).Concat(GetSpectatorsFromTournament(tournament)).ToList();
        }


        private async Task CleanupUserFromOnlineUsersList(User user)
        {
            _userRepository.RemoveUser(user);
            await GetAllOnlineUsers();
        }

        private async Task SendMessage(string message, TypeOfMessage typeOfMessage, ChatDestination chatDestination, User user)
        {
            var gameId = user.ActiveGameId;
            var tournamentId = user.ActiveTournamentId;

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
                allUsersInTournament = GetContestantsAndSpectatorsFromTournament(tournament);
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
                    if (!string.IsNullOrEmpty(user.ActiveGameId))
                        await Clients.Caller.SendAsync("PostNewMessage", msgDto, ChatDestination.Game);
                    if (!string.IsNullOrEmpty(user.ActiveTournamentId))
                        await Clients.Caller.SendAsync("PostNewMessage", msgDto, ChatDestination.Tournament);
                    await Clients.Caller.SendAsync("PostNewMessage", msgDto, ChatDestination.All);
                }
                else
                {
                    await Clients.Clients(allUsersInGame).SendAsync("PostNewMessage", msgDto, ChatDestination.Game);
                    await Clients.Clients(allUsersInTournament).SendAsync("PostNewMessage", msgDto, ChatDestination.Tournament);
                    await Clients.All.SendAsync("PostNewMessage", msgDto, ChatDestination.All);
                    if (!string.IsNullOrEmpty(targetedUser.ActiveGameId) && user.ActiveGameId != targetedUser.ActiveGameId)
                    {
                        var targetedPlayersGame = _gameRepository.GetGameByGameId(targetedUser.ActiveGameId);
                        var allUsersInTargetedUsersGame = GetPlayersAndSpectatorsFromGame(targetedPlayersGame);
                        await Clients.Clients(allUsersInTargetedUsersGame).SendAsync("PostNewMessage", msgDto, ChatDestination.Game);

                    }
                    if (!string.IsNullOrEmpty(targetedUser.ActiveTournamentId) && user.ActiveTournamentId != targetedUser.ActiveTournamentId)
                    {
                        var targetedPlayersTournament = _tournamentRepository.GetTournament(targetedUser.ActiveTournamentId);
                        var allUsersInTargetedUsersTournament = GetContestantsAndSpectatorsFromTournament(targetedPlayersTournament);
                        await Clients.Clients(allUsersInTargetedUsersTournament).SendAsync("PostNewMessage", msgDto, ChatDestination.Tournament);
                    }
                }

            }
            else if (chatMessageIntentionResult.ChatMessageIntention == ChatMessageIntention.Normal)
            {
                var msg = new ChatMessage(username, message, typeOfMessage);
                msgDto = _mapper.Map<ChatMessageDto>(msg);
                if (chatDestination == ChatDestination.Game)
                {
                    game.ChatMessages.Add(msg);
                    await Clients.Clients(allUsersInGame).SendAsync("PostNewMessage", msgDto, ChatDestination.Game);
                }
                else if (chatDestination == ChatDestination.Tournament)
                {
                    tournament.ChatMessages.Add(msg);
                    await Clients.Clients(allUsersInTournament).SendAsync("PostNewMessage", msgDto, ChatDestination.Tournament);
                }
                else
                {
                    await Clients.All.SendAsync("PostNewMessage", msgDto, ChatDestination.All);
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

        private async Task ExitTournament(User user)
        {
            var tournamentId = user.ActiveTournamentId;

            if (string.IsNullOrEmpty(tournamentId))
                return;

            var tournament = _tournamentRepository.GetTournament(tournamentId);

            await SendMessage($"{user.Name} has left the tournament.", TypeOfMessage.Server, ChatDestination.Tournament, user);

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


            await Clients.Clients(user.ConnectionId).SendAsync("ExitTournament");
            await UpdateTournament(tournament);

            if (!GetContestantsAndSpectatorsFromTournament(tournament).Any())
            {
                tournament.TournamentRounds.ForEach(x =>
                {
                    x.TournamentRoundGames.ForEach(y => { _gameRepository.RemoveGame(y.Game); });
                });
                _tournamentRepository.RemoveTournament(tournament);
            }

            user.ActiveTournamentId = string.Empty;

            await GetAllTournaments();
        }

        private async Task ExitGame(User user)
        {
            var gameId = user.ActiveGameId;

            if (string.IsNullOrEmpty(gameId))
                return;

            var game = _gameRepository.GetGameByGameId(gameId);
            var allPlayersFromGame = GetPlayersFromGame(game);

            if (allPlayersFromGame.Contains(user.ConnectionId))
            {
                var player = game.Players.First(y => y.User.ConnectionId == user.ConnectionId);
                if (game.GameStarted)
                {
                    player.LeftGame = true;
                    await DisplayToastMessageToGame(gameId, $"User {player.User.Name} has left the game.", "info");
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
            await SendMessage($"{user.Name} has left the game.", TypeOfMessage.Server, ChatDestination.Game, user);

            if (game.Players.All(x => x.LeftGame) && !game.Spectators.Any() && !game.IsTournamentGame)
            {
                _gameRepository.RemoveGame(game);
            }

            user.ActiveGameId = string.Empty;

            await GetAllGames();
            await Clients.Client(user.ConnectionId).SendAsync("ExitGame");
        }

        private User GetCurrentUser()
        {
            return _userRepository.GetUserByConnectionId(Context.ConnectionId);
        }


        #endregion

    }
}