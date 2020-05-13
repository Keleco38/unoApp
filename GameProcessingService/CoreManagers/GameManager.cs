using System;
using System.Collections.Generic;
using System.Linq;
using Common.Enums;
using Common.Extensions;
using EntityObjects;
using EntityObjects.Cards.Abstraction;
using GameProcessingService.Models;
using Repository;

namespace GameProcessingService.CoreManagers
{
    public class GameManager : IGameManager
    {

        public void StartNewGame(Game game)
        {
            game.Players.Shuffle();

            if (game.GameSetup.PlayersSetup == PlayersSetup.Teams)
            {
                game.Players = game.Players.OrderBy(x => x.TeamNumber).ToList();
                var sortedCorrectly = new List<Player>();
                while (game.Players.Any())
                {
                    var temporaryList = new List<Player>();
                    game.Players.ForEach(x =>
                    {
                        if (temporaryList.FirstOrDefault(y => y.TeamNumber == x.TeamNumber) == null)
                        {
                            temporaryList.Add(x);
                        }
                    });
                    temporaryList.ForEach(x => game.Players.Remove(x));
                    sortedCorrectly.AddRange(temporaryList);
                }
                game.Players = sortedCorrectly;
            }

            Random random = new Random();
            game.GameStarted = true;
            game.Direction = Direction.Right;
            StartNewRound(game);
            game.PlayerToPlay = game.Players[random.Next(game.Players.Count)];

        }

        public void StartNewRound(Game game)
        {
            game.GreedAffectedPlayers.Clear();
            game.CardValuesRemovedFromTheRound.Clear();
            game.HandCuffedPlayers.Clear();
            game.SilenceTurnsRemaining = 0;
            game.Players.ForEach(x => x.CardPromisedToDiscard = null);
            game.Players.ForEach(x => x.MustCallUno = false);
            game.DiscardedPile.Clear();
            game.Deck = new Deck(game.GameSetup);
            game.Deck.Cards.Shuffle();
            ICard lastCardDrew;
            do
            {
                lastCardDrew = game.Deck.Draw(1).First();
                game.DiscardedPile.Add(lastCardDrew);
            } while (lastCardDrew.Color == CardColor.Wild);
            game.LastCardPlayed = new LastCardPlayed(lastCardDrew.Color, lastCardDrew.Value, lastCardDrew.ImageUrl, string.Empty, false, lastCardDrew);
            game.Players.ForEach(x => x.Cards = game.Deck.Draw(7));
            game.DrawAutoPlayPlayer = null;
            game.DrawAutoPlayCard = null;
        }

        public void DrawCard(Game game, Player player, int count, bool normalDraw)
        {
            if (game.Deck.Cards.Count < count)
            {
                game.Deck = new Deck(game.GameSetup);
                game.Deck.Cards.Shuffle();
                if (game.CardValuesRemovedFromTheRound.Any())
                {
                    foreach (var cardValue in game.CardValuesRemovedFromTheRound)
                    {
                        game.Deck.Cards.RemoveAll(x => x.Value == cardValue);
                    }
                }
            }

            player.Cards.AddRange(game.Deck.Draw(count));

            if (normalDraw)
            {
                // if it's normalDraw then it's not a result of a wildcard
                game.PlayerToPlay = GetNextPlayer(game, game.PlayerToPlay, game.Players);
            }
        }

        public Player GetNextPlayer(Game game, Player player, List<Player> listOfPlayers, bool flipDirection = false)
        {
            var direction = game.Direction;

            if (flipDirection)
            {
                direction = game.Direction == Direction.Right ? Direction.Left : Direction.Right;
            }


            var indexOfCurrentPlayer = listOfPlayers.IndexOf(player);
            if (indexOfCurrentPlayer == -1)
            {
                indexOfCurrentPlayer = 0;
            }
            if (direction == Direction.Right)
            {
                if (indexOfCurrentPlayer == listOfPlayers.Count - 1)
                {
                    return listOfPlayers.First();
                }
                else
                {
                    return listOfPlayers[indexOfCurrentPlayer + 1];
                }
            }
            if (direction == Direction.Left)
            {
                if (indexOfCurrentPlayer == 0)
                {
                    return listOfPlayers.Last();
                }
                else
                {
                    return listOfPlayers[indexOfCurrentPlayer - 1];
                }
            }
            throw new Exception("Error, can't access that direction");
        }

        public List<string> UpdateGameAndRoundStatus(Game game)
        {
            var messages = new List<string>();

            var playersWithoutCards = game.Players.Where(x => !x.Cards.Any()).ToList();
            if (game.GameSetup.PlayersSetup == PlayersSetup.Teams)
            {
                var teamsWon = playersWithoutCards.Select(x => x.TeamNumber).Distinct().ToList();
                playersWithoutCards = game.Players.Where(x => teamsWon.Contains(x.TeamNumber)).ToList();
            }
            if (playersWithoutCards.Any())
            {
                foreach (var player in playersWithoutCards)
                {
                    player.RoundsWonCount++;
                }

                messages.Add($"Round ended! Players that won that round: {string.Join(',', playersWithoutCards.Select(x => x.User.Name.ToUpper() + " [ " + x.RoundsWonCount + " / " + game.GameSetup.RoundsToWin + "  ]"))}");


                var playersThatMatchWinCriteria = game.Players.Where(x => x.RoundsWonCount == game.GameSetup.RoundsToWin).ToList();
                if (playersThatMatchWinCriteria.Any())
                {
                    if (game.IsTournamentGame && playersThatMatchWinCriteria.Count > 1)
                    {
                        game.GameSetup.RoundsToWin += 1;
                        messages.Add($"Game ended! Both players reached the winning condition. Adding one extra round (game must have a winner). Playing until: " + game.GameSetup.RoundsToWin);
                        game.RoundEnded = true;
                        StartNewRound(game);
                    }
                    else
                    {
                        game.GameEnded = true;
                        messages.Add($"Game ended! Players that won the game: {string.Join(',', playersThatMatchWinCriteria.Select(x => x.User.Name.ToUpper()))}");
                    }

                }
                else
                {
                    game.RoundEnded = true;
                    StartNewRound(game);
                }
            }

            return messages;
        }
    }
}