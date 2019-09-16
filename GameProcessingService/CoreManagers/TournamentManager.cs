using System;
using System.Linq;
using Common.Enums;
using Common.Extensions;
using EntityObjects;
using Repository;

namespace GameProcessingService.CoreManagers
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
            tournament.TournamentStarted = true;
            tournament.Contestants.Shuffle();
            var numberOfRounds = (int)(Math.Log(tournament.Contestants.Count) / Math.Log(2));
            var numberOfGamesInARound = tournament.Contestants.Count / 2;

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
                        WildCardCanBePlayedOnlyIfNoOtherOptions = tournament.TournamentSetup.WildCardCanBePlayedOnlyIfNoOtherOptions
                    }, tournament.Id);
                    _gameRepository.AddGame(game);
                    var tournamentRoundGame = new TournamentRoundGame(j, game);
                    if (i == 1)
                    {
                        for (int k = 0; k < 2; k++)
                        {
                            var player = new Player(tournament.Contestants[(j - 1) * 2 + k].User) { LeftGame = true };
                            tournamentRoundGame.Game.Players.Add(player);
                            if (k == 1)
                                _gameManager.StartNewGame(tournamentRoundGame.Game);
                        }
                    }
                    tournamentRound.TournamentRoundGames.Add(tournamentRoundGame);
                }

                numberOfGamesInARound = numberOfGamesInARound / 2;
                tournament.TournamentRounds.Add(tournamentRound);
            }
        }

        public void UpdateTournament(Tournament tournament, Game gameEnded)
        {
            int totalNumberOfRounds = (int)(Math.Log(tournament.Contestants.Count) / Math.Log(2));
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
                gameInTournament.Players.Add(new Player(playerWon.User) { LeftGame = true });
                if (gameInTournament.Players.Count == 2)
                {
                    _gameManager.StartNewGame(gameInTournament);
                }
            }
            else
            {
                tournament.TournamentWinner = gameEnded.Players.Single(x => !x.Cards.Any()).User.Name;
                tournament.TournamentEnded = true;
            }
        }
    }
}