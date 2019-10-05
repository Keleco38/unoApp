using System;
using System.Collections.Generic;
using System.Linq;
using Common.Enums;
using Common.Extensions;
using EntityObjects;
using GameProcessingService.CoreManagers;
using Repository;

namespace PreMoveProcessingService.CoreManagers
{
    public class TournamentManager : ITournamentManager
    {
        private readonly IGameManager _gameManager;
        private readonly IGameRepository _gameRepository;

        public TournamentManager(IGameManager gameManager, IGameRepository gameRepository)
        {
            _gameManager = gameManager;
            _gameRepository = gameRepository;
        }
        public void StartTournament(Tournament tournament)
        {
            tournament.Contestants.Shuffle();

            var paddedNumberOfPlayers = Math.Pow(2, (int)Math.Ceiling((Math.Log(tournament.Contestants.Count) / Math.Log(2))));

            var numberOfRounds = (int)(Math.Log(paddedNumberOfPlayers) / Math.Log(2));
            var numberOfGamesInARound = paddedNumberOfPlayers / 2;

            for (int i = 1; i <= numberOfRounds; i++)
            {
                TournamentRound tournamentRound = new TournamentRound(i);

                for (int j = 1; j <= numberOfGamesInARound; j++)
                {
                    var game = new Game(new GameSetup()
                    {
                        PlayersSetup = PlayersSetup.Individual,
                        RoundsToWin = tournament.TournamentSetup.RoundsToWin,
                        GameType = tournament.TournamentSetup.GameType,
                        BannedCards = tournament.TournamentSetup.BannedCards,
                        DrawFourDrawTwoShouldSkipTurn = tournament.TournamentSetup.DrawFourDrawTwoShouldSkipTurn,
                        MatchingCardStealsTurn = tournament.TournamentSetup.MatchingCardStealsTurn,
                        MaxNumberOfPlayers = 2,
                        Password = tournament.TournamentSetup.Password,
                        ReverseShouldSkipTurnInTwoPlayers = tournament.TournamentSetup.ReverseShouldSkipTurnInTwoPlayers,
                        WildCardCanBePlayedOnlyIfNoOtherOptions = tournament.TournamentSetup.WildCardCanBePlayedOnlyIfNoOtherOptions,
                        CanSeeTeammatesHandInTeamGame = false,
                        DrawAutoPlay = tournament.TournamentSetup.DrawAutoPlay,
                        SpectatorsCanViewHands = false
                    }, tournament.Id);
                    _gameRepository.AddGame(game);

                    var tournamentRoundGame = new TournamentRoundGame(j, game) { Game = { Players = new List<Player>(2) } };
                    tournamentRound.TournamentRoundGames.Add(tournamentRoundGame);
                }
                numberOfGamesInARound /= 2;
                tournament.TournamentRounds.Add(tournamentRound);
            }


            for (int i = 0; i < tournament.TournamentRounds[0].TournamentRoundGames.Count; i++)
            {
                tournament.TournamentRounds[0].TournamentRoundGames[i].Game.Players.Add(new Player(tournament.Contestants[i].User, 1) { LeftGame = true });
                if (tournament.Contestants.ElementAtOrDefault(tournament.TournamentRounds[0].TournamentRoundGames.Count + i) != null)
                {
                    tournament.TournamentRounds[0].TournamentRoundGames[i].Game.Players.Add(new Player(tournament.Contestants[tournament.TournamentRounds[0].TournamentRoundGames.Count + i].User, 2) { LeftGame = true });

                    tournament.TournamentRounds[0].TournamentRoundGames[i].Game.GameLog.Add("Game started.");
                    tournament.TournamentRounds[0].TournamentRoundGames[i].Game.GameLog.Add("If you need more detailed log info, press the 'Game info' button.");
                    tournament.TournamentRounds[0].TournamentRoundGames[i].Game.GameLog.Add("This is the game log summary. We will display the last 3 entries here.");

                    _gameManager.StartNewGame(tournament.TournamentRounds[0].TournamentRoundGames[i].Game);
                }
                else
                {
                    tournament.TournamentRounds[0].TournamentRoundGames[i].Game.GameEnded = true;
                    tournament.TournamentRounds[0].TournamentRoundGames[i].Game.Players.First().RoundsWonCount = tournament.TournamentSetup.RoundsToWin;
                    UpdateTournament(tournament, tournament.TournamentRounds[0].TournamentRoundGames[i].Game);
                }

            }
            
            tournament.TournamentStarted = true;

        }

        public void UpdateTournament(Tournament tournament, Game gameEnded)
        {
            var paddedNumberOfPlayers = Math.Pow(2, (int)Math.Ceiling((Math.Log(tournament.Contestants.Count) / Math.Log(2))));
            int totalNumberOfRounds = (int)(Math.Log(paddedNumberOfPlayers) / Math.Log(2));
            int roundNumber = 0;
            int gameNumberInRound = 0;

            tournament.TournamentRounds.ForEach(r =>
            {
                r.TournamentRoundGames.ForEach(rg =>
                {
                    if (rg.Game.Id == gameEnded.Id)
                    {
                        roundNumber = r.RoundNumber;
                        gameNumberInRound = rg.GameNumber;
                    }
                });
            });

            if (roundNumber == 0 || gameNumberInRound == 0)
                return;

            if (roundNumber != totalNumberOfRounds)
            {
                var roundNumberToAddNewPlayer = roundNumber + 1;
                var gameNumberToAddNewPlayer = (int)Math.Ceiling((float)gameNumberInRound / 2);
                var gameInTournament = tournament.TournamentRounds[roundNumberToAddNewPlayer - 1].TournamentRoundGames[gameNumberToAddNewPlayer - 1].Game;
                var playerWon = gameEnded.Players.First(x => x.RoundsWonCount == gameEnded.GameSetup.RoundsToWin);

                var placeInNewGame = gameNumberInRound % 2 == 0 ? 2 : 1;

                gameInTournament.Players.Add(new Player(playerWon.User, placeInNewGame) { LeftGame = true });
                if (gameInTournament.Players.Count == 2)
                {
                    gameInTournament.GameLog.Add("Game started.");
                    gameInTournament.GameLog.Add("If you need more detailed log info, press the 'Game info' button.");
                    gameInTournament.GameLog.Add("This is the game log summary. We will display the last 3 entries here.");
                    _gameManager.StartNewGame(gameInTournament);
                }
            }
            else
            {
                tournament.TournamentWinner = gameEnded.Players.First(x => !x.Cards.Any()).User.Name;
                tournament.TournamentEnded = true;
            }
        }
    }
}