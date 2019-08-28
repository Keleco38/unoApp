using System;
using System.Collections.Generic;
using System.Linq;
using Common.Enums;
using EntityObjects;
using EntityObjects.Cards.Abstraction;
using GameProcessingService.CardEffectProcessors;
using GameProcessingService.Models;

namespace GameProcessingService.GameManager
{
    public class GameManager : IGameManager
    {
        public void StartNewGame(Game game)
        {
            game.Players.ForEach(x => x.CardPromisedToDiscard = null);
            Random random = new Random();
            ICard lastCardDrew;
            game.DiscardedPile = new List<ICard>();
            game.Deck = new Deck(game.GameSetup.BannedCards);
            do
            {
                lastCardDrew = game.Deck.Draw(1).First();
                game.DiscardedPile.Add(lastCardDrew);
            } while (lastCardDrew.Color == CardColor.Wild);
            game.LastCardPlayed = new LastCardPlayed(lastCardDrew.Color, lastCardDrew.Value, lastCardDrew.ImageUrl, string.Empty, false);
            game.Direction = Direction.Right;
            game.PlayerToPlay = game.Players[random.Next(game.Players.Count)];
            game.Players.ForEach(x => x.Cards = game.Deck.Draw(7));
            game.GameStarted = true;
            game.RoundEnded = false;
        }

        public void DrawCard(Game game, Player player, int count, bool normalDraw)
        {
            var deckCount = game.Deck.Cards.Count;
            if (deckCount < count)
            {
                player.Cards.AddRange(game.Deck.Draw(deckCount));
                game.Deck.Cards = game.DiscardedPile.ToList();
                game.Deck.Shuffle();
                game.DiscardedPile.RemoveRange(0, game.DiscardedPile.Count - 1);
                player.Cards.AddRange(game.Deck.Draw(count - deckCount));
            }
            else
            {
                player.Cards.AddRange(game.Deck.Draw(count));
            }

            if (normalDraw)
            {
                // if it's normalDraw then it's not a result of a wildcard
                game.PlayerToPlay = GetNextPlayer(game, game.PlayerToPlay, game.Players);
            }
        }

        public Player GetNextPlayer(Game game, Player player, List<Player> listOfPlayers)
        {
            var indexOfCurrentPlayer = listOfPlayers.IndexOf(player);
            if (indexOfCurrentPlayer == -1)
            {
                indexOfCurrentPlayer = 0;
            }
            if (game.Direction == Direction.Right)
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
            if (game.Direction == Direction.Left)
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

        // -------------------------------------private------------

        public void UpdateGameAndRoundStatus(Game game, MoveResult moveResult)
        {
            var playersWithoutCards = game.Players.Where(x => !x.Cards.Any());
            if (playersWithoutCards.Any())
            {
                var firstPlayerWithTheLastStand = game.Players.Where(x => x.Cards.Any()).FirstOrDefault(x => x.Cards.FirstOrDefault(y => y.Value == CardValue.TheLastStand) != null);

                if (firstPlayerWithTheLastStand != null)
                {
                    var theLastStandCard = firstPlayerWithTheLastStand.Cards.First(x => x.Value == CardValue.TheLastStand);
                    game.LastCardPlayed = new LastCardPlayed(game.LastCardPlayed.Color, theLastStandCard.Value, theLastStandCard.ImageUrl, game.PlayerToPlay.User.Name, true);
                    firstPlayerWithTheLastStand.Cards.Remove(theLastStandCard);
                    game.DiscardedPile.Add(theLastStandCard);
                    moveResult.MessagesToLog.Add($"{firstPlayerWithTheLastStand.User.Name} saved the day! They played The Last Stand. Every player that had 0 cards will draw 2 cards.");
                    foreach (var player in playersWithoutCards)
                    {
                        DrawCard(game, player, 2, false);
                    }
                    return;
                }

                foreach (var player in playersWithoutCards)
                {
                    player.RoundsWonCount++;
                }
                game.RoundEnded = true;
                moveResult.MessagesToLog.Add($"Round ended! Players that won that round: {string.Join(',', playersWithoutCards.Select(x => x.User.Name))}");
            }
            var playersThatMatchWinCriteria = game.Players.Where(x => x.RoundsWonCount == game.GameSetup.RoundsToWin).ToList();
            if (playersThatMatchWinCriteria.Any())
            {
                game.GameEnded = true;
                moveResult.MessagesToLog.Add($"Game ended! Players that won the game: {string.Join(',', playersThatMatchWinCriteria.Select(x => x.User.Name))}");
            }
        }
    }
}