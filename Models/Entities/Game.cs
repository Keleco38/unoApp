using System;
using System.Collections.Generic;
using System.Linq;
using Uno.Enums;
using Uno.Models.Entities.Cards.Abstraction;
using Uno.Models.Helpers;

namespace Uno.Models.Entities
{
    public class Game
    {
        public string Id { get; set; }
        public Deck Deck { get; set; }
        public List<Player> Players { get; set; }
        public List<Spectator> Spectators { get; set; }
        public List<ICard> DiscardedPile { get; set; }
        public GameSetup GameSetup { get; set; }
        public Direction Direction { get; set; }
        public LastCardPlayed LastCardPlayed { get; set; }
        public Player PlayerToPlay { get; set; }
        public bool GameStarted { get; set; }
        public bool RoundEnded { get; set; }
        public bool GameEnded { get; set; }

        public Game()
        {
            Id = Guid.NewGuid().ToString();
            GameSetup = new GameSetup();
            Players = new List<Player>();
            Spectators = new List<Spectator>();
        }

        public MoveResult PlayCard(Player playerPlayed, string cardPlayedId, CardColor targetedCardColor, string playerTargetedId, string cardToDigId, List<int> duelNumbers,
            List<string> charityCardsIds, int blackjackNumber, List<int> numbersToDiscard, string cardPromisedToDiscardId, string oddOrEvenGuess)
        {
            var cardPlayed = playerPlayed.Cards.Find(x => x.Id == cardPlayedId);

            if (PlayerToPlay != playerPlayed && cardPlayed.Value != CardValue.StealTurn)
                return null;
            if (cardPlayed.Color != CardColor.Wild && cardPlayed.Color != LastCardPlayed.Color && cardPlayed.Value != LastCardPlayed.Value)
                return null;

            playerPlayed.Cards.Remove(cardPlayed);
            DiscardedPile.Add(cardPlayed);

            var extraMessageToLog = string.Empty;

            if (playerPlayed.CardPromisedToDiscard != null)
            {
                if (cardPlayed.Color == playerPlayed.CardPromisedToDiscard.Color && cardPlayed.Value == playerPlayed.CardPromisedToDiscard.Value)
                {
                    extraMessageToLog = $"{playerPlayed.User.Name} fulfilled their promise, they will discard one card. ";
                    if (playerPlayed.Cards.Count > 0)
                    {
                        playerPlayed.Cards.RemoveRange(0, 1);
                    }
                }
                else
                {
                    extraMessageToLog = $"{playerPlayed.User.Name} didn't fulfill their promise, they will draw 2 cards. ";
                    DrawCard(playerPlayed, 2, false);
                }
                playerPlayed.CardPromisedToDiscard = null;
            }

            var playerTargeted = string.IsNullOrEmpty(playerTargetedId) ? GetNextPlayer(playerPlayed, Players) : Players.Find(x => x.Id == playerTargetedId);
            var colorForLastCard = targetedCardColor == 0 ? cardPlayed.Color : targetedCardColor;

            LastCardPlayed = new LastCardPlayed(colorForLastCard, cardPlayed.Value, cardPlayed.ImageUrl, playerPlayed.User.Name, cardPlayed.Color == CardColor.Wild);

            var cardToDig = string.IsNullOrEmpty(cardToDigId) ? null : DiscardedPile.Find(x => x.Id == cardToDigId);
            var cardPromisedToDiscard = string.IsNullOrEmpty(cardPromisedToDiscardId) ? null : playerPlayed.Cards.Find(x => x.Id == cardPromisedToDiscardId);
            var charityCards = charityCardsIds != null ? playerPlayed.Cards.Where(x => charityCardsIds.Contains(x.Id)).ToList() : null;

            var moveParams = new MoveParams(playerPlayed, playerTargeted, colorForLastCard, cardToDig, duelNumbers, charityCards, blackjackNumber, numbersToDiscard, cardPromisedToDiscard, oddOrEvenGuess);

            var moveResult = cardPlayed.ProcessCardEffect(this, moveParams);
            if (!string.IsNullOrEmpty(extraMessageToLog))
            {
                moveResult.MessagesToLog.Add(extraMessageToLog);
            }

            UpdateGameAndRoundStatus(moveResult);
            if (GameEnded)
            {
                return moveResult;
            }
            if (RoundEnded)
            {
                StartNewGame();
                return moveResult;
            }

            PlayerToPlay = GetNextPlayer(PlayerToPlay, Players);
            return moveResult;
        }



        public void StartNewGame()
        {
            Players.ForEach(x => x.CardPromisedToDiscard = null);
            Random random = new Random();
            ICard lastCardDrew;
            DiscardedPile = new List<ICard>();
            Deck = new Deck(GameSetup.BannedCards);
            do
            {
                lastCardDrew = Deck.Draw(1).First();
                DiscardedPile.Add(lastCardDrew);
            } while (lastCardDrew.Color == CardColor.Wild);
            LastCardPlayed = new LastCardPlayed(lastCardDrew.Color, lastCardDrew.Value, lastCardDrew.ImageUrl, string.Empty, false);
            Direction = Direction.Right;
            PlayerToPlay = Players[random.Next(Players.Count)];
            Players.ForEach(x => x.Cards = Deck.Draw(7));
            GameStarted = true;
            RoundEnded = false;
        }


        public void DrawCard(Player player, int count, bool normalDraw)
        {
            var deckCount = Deck.Cards.Count;
            if (deckCount < count)
            {
                player.Cards.AddRange(Deck.Draw(deckCount));
                Deck.Cards = DiscardedPile.ToList();
                Deck.Shuffle();
                DiscardedPile.RemoveRange(0, DiscardedPile.Count - 1);
                player.Cards.AddRange(Deck.Draw(count - deckCount));
            }
            else
            {
                player.Cards.AddRange(Deck.Draw(count));
            }

            if (normalDraw)
            {
                // if it's normalDraw then it's not a result of a wildcard
                PlayerToPlay = GetNextPlayer(PlayerToPlay, Players);
            }
        }




        public Player GetNextPlayer(Player player, List<Player> ListOfPlayers)
        {
            var indexOfCurrentPlayer = ListOfPlayers.IndexOf(player);
            if (indexOfCurrentPlayer == -1)
            {
                indexOfCurrentPlayer = 0;
            }
            if (Direction == Direction.Right)
            {
                if (indexOfCurrentPlayer == ListOfPlayers.Count - 1)
                {
                    return ListOfPlayers.First();
                }
                else
                {
                    return ListOfPlayers[indexOfCurrentPlayer + 1];
                }
            }
            if (Direction == Direction.Left)
            {
                if (indexOfCurrentPlayer == 0)
                {
                    return ListOfPlayers.Last();
                }
                else
                {
                    return ListOfPlayers[indexOfCurrentPlayer - 1];
                }
            }
            throw new Exception("Error, can't access that direction");

        }

        // -------------------------------------private------------

        private void UpdateGameAndRoundStatus(MoveResult moveResult)
        {
            var playersWithoutCards = Players.Where(x => !x.Cards.Any());
            if (playersWithoutCards.Any())
            {
                var firstPlayerWithTheLastStand = Players.Where(x => x.Cards.Any()).FirstOrDefault(x => x.Cards.FirstOrDefault(y => y.Value == CardValue.TheLastStand) != null);

                if (firstPlayerWithTheLastStand != null)
                {
                    var theLastStandCard = firstPlayerWithTheLastStand.Cards.First(x => x.Value == CardValue.TheLastStand);
                    LastCardPlayed = new LastCardPlayed(LastCardPlayed.Color, theLastStandCard.Value, theLastStandCard.ImageUrl, PlayerToPlay.User.Name, true);
                    firstPlayerWithTheLastStand.Cards.Remove(theLastStandCard);
                    DiscardedPile.Add(theLastStandCard);
                    moveResult.MessagesToLog.Add($"{firstPlayerWithTheLastStand.User.Name} saved the day! They played The Last Stand. Every player that had 0 cards will draw 2 cards.");
                    foreach (var player in playersWithoutCards)
                    {
                        DrawCard(player, 2, false);
                    }
                    return;
                }

                foreach (var player in playersWithoutCards)
                {
                    player.RoundsWonCount++;
                }
                RoundEnded = true;
                moveResult.MessagesToLog.Add($"Round ended! Players that won that round: {string.Join(',', playersWithoutCards.Select(x => x.User.Name))}");
            }
            var playersThatMatchWinCriteria = Players.Where(x => x.RoundsWonCount == GameSetup.RoundsToWin);
            if (playersThatMatchWinCriteria.Any())
            {
                GameEnded = true;
                moveResult.MessagesToLog.Add($"Game ended! Players that won the game: {string.Join(',', playersThatMatchWinCriteria.Select(x => x.User.Name))}");
            }
        }
    }
}