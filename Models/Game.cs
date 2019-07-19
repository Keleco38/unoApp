using System;
using System.Collections.Generic;
using System.Linq;
using Uno.Contants;
using Uno.Enums;
using unoApp.Models.Helpers;

namespace Uno.Models
{
    public class Game
    {
        public Deck Deck { get; set; }
        public List<Player> Players { get; set; }
        public List<Spectator> Spectators { get; set; }
        public List<Card> DiscardedPile { get; set; }
        public GameSetup GameSetup { get; set; }
        public Direction Direction { get; set; }
        public LastCardPlayed LastCardPlayed { get; set; }
        public Player PlayerToPlay { get; set; }
        public bool GameStarted { get; set; }
        public bool RoundEnded { get; set; }
        public bool GameEnded { get; set; }

        public Game(GameSetup gameSetup)
        {
            GameSetup = gameSetup;
            Players = new List<Player>();
            Spectators = new List<Spectator>();
            DiscardedPile = new List<Card>();
        }

        public TurnResult PlayCard(Player player, Card cardPlayed, CardColor pickedCardColor, string targetedPlayerName)
        {
            var turnResult = new TurnResult();

            var card = player.Cards.Find(y => y.Color == cardPlayed.Color && y.Value == cardPlayed.Value);

            if (PlayerToPlay != player && card.Value != CardValue.StealTurn)
                return turnResult;
            if (card.Color != CardColor.Wild && card.Color != LastCardPlayed.Color && card.Value != LastCardPlayed.Value)
                return turnResult;

            turnResult.Success = true;

            player.Cards.Remove(card);
            DiscardedPile.Add(card);

            LastCardPlayed = new LastCardPlayed(pickedCardColor, card.Value, card.ImageUrl, player.User.Name);



            if (card.Color == CardColor.Wild)
            {
                if (card.Value == CardValue.DrawFour)
                {
                    var targetedPlayer = GetNextPlayerToPlay();
                    var messageToLog = $"Player {player.User.Name} targeted player {targetedPlayer.User.Name} with +4 card.";
                    var deflectCard = targetedPlayer.Cards.FirstOrDefault(x => x.Value == CardValue.Deflect);
                    if (deflectCard == null)
                    {
                        messageToLog += $"Player {targetedPlayer.User.Name} drew 4 cards.";
                        DrawCard(targetedPlayer, 4, false);
                    }
                    else
                    {
                        LastCardPlayed = new LastCardPlayed(pickedCardColor, deflectCard.Value, deflectCard.ImageUrl, targetedPlayer.User.Name);
                        targetedPlayer.Cards.Remove(deflectCard);
                        DiscardedPile.Add(deflectCard);
                        DrawCard(player, 4, false);
                        messageToLog += $"Player {targetedPlayer.User.Name} has auto deflected +4 card. {player.User.Name} must draw 4 cards.";

                    }
                    turnResult.MessagesToLog.Add(messageToLog);
                }
                else if (card.Value == CardValue.DiscardWildCards)
                {
                    Players.ForEach(x =>
                    {
                        var wildCards = x.Cards.Where(y => y.Color == CardColor.Wild).ToList();
                        DiscardedPile.AddRange(wildCards);
                        wildCards.ForEach(y => x.Cards.Remove(y));
                    });
                    turnResult.MessagesToLog.Add($"Player {player.User.Name} played discard all wildcards.");
                }
                else if (card.Value == CardValue.SwapHands)
                {
                    var targetedPlayer = Players.Find(x => x.User.Name == targetedPlayerName);

                    var playersCards = PlayerToPlay.Cards.ToList();
                    var targetedPlayerCards = targetedPlayer.Cards.ToList();

                    PlayerToPlay.Cards = targetedPlayerCards;
                    targetedPlayer.Cards = playersCards;

                    turnResult.MessagesToLog.Add($"Player {player.User.Name} has swapped hands with {targetedPlayer.User.Name}");
                }
                else if (card.Value == CardValue.BlackHole)
                {
                    turnResult.MessagesToLog.Add($"Player {player.User.Name} played black hole card.");
                    Players.ForEach(x =>
                    {
                        var cardCount = x.Cards.Count;
                        DiscardedPile.AddRange(x.Cards.ToList());
                        x.Cards.Clear();
                        DrawCard(x, cardCount, false);
                    });
                }
                else if (card.Value == CardValue.DoubleEdge)
                {
                    var targetedPlayer = Players.Find(x => x.User.Name == targetedPlayerName);

                    var messageToLog = $"Player {player.User.Name} has targeted {targetedPlayer.User.Name} with double edge card.";

                    var deflectCard = targetedPlayer.Cards.FirstOrDefault(x => x.Value == CardValue.Deflect);
                    if (deflectCard == null)
                    {
                        messageToLog += $"Player {targetedPlayer.User.Name} drew 5 cards.";
                        messageToLog += $"Player {player.User.Name} drew 2 cards.";
                        DrawCard(targetedPlayer, 5, false);
                        DrawCard(PlayerToPlay, 2, false);
                    }
                    else
                    {
                        LastCardPlayed = new LastCardPlayed(pickedCardColor, deflectCard.Value, deflectCard.ImageUrl, targetedPlayer.User.Name);
                        targetedPlayer.Cards.Remove(deflectCard);
                        DiscardedPile.Add(deflectCard);
                        DrawCard(PlayerToPlay, 5, false);
                        DrawCard(targetedPlayer, 2, false);
                        messageToLog += $"Player {targetedPlayer.User.Name} has auto deflected double edge card. He will draw 2 cards and {player.User.Name} must draw 5 cards.";
                    }
                    turnResult.MessagesToLog.Add(messageToLog);

                }
                else if (card.Value == CardValue.DiscardColor)
                {
                    Players.ForEach(x =>
                    {
                        var cardsInThatColor = x.Cards.Where(y => y.Color == pickedCardColor).ToList();
                        DiscardedPile.AddRange(cardsInThatColor);
                        cardsInThatColor.ForEach(y => x.Cards.Remove(y));

                    });
                    // now randomize color
                    Random random = new Random();
                    var numbers = new int[] { 1, 2, 3, 4 };
                    int randomColor = numbers[(random.Next(4))];
                    LastCardPlayed.Color = (CardColor)randomColor;
                    turnResult.MessagesToLog.Add($"Player {player.User.Name} has played discard color card. All players discarded {pickedCardColor} and a random color has been assigned.");

                }
                else if (card.Value == CardValue.HandOfGod)
                {
                    if (player.Cards.Count > 7)
                    {
                        turnResult.MessagesToLog.Add($"Player {player.User.Name} discardeed 4 cards (hand of god).");
                        var cards = player.Cards.Take(4).ToList();
                        DiscardedPile.AddRange(cards);
                        cards.ForEach(y => player.Cards.Remove(y));
                    }
                    else
                    {
                        turnResult.MessagesToLog.Add($"Player {player.User.Name} didn't discard any cards. He had less than 8 cards. (hand of god)");
                    }
                }
                else if (card.Value == CardValue.Judgement)
                {
                    var targetedPlayer = Players.Find(x => x.User.Name == targetedPlayerName);
                    var messageToLog = $"Player {player.User.Name} has targeted player {targetedPlayer.User.Name} with the judgement card.";
                    if (targetedPlayer.Cards.Any(x => x.Color == CardColor.Wild))
                    {
                        var deflectCard = targetedPlayer.Cards.FirstOrDefault(x => x.Value == CardValue.Deflect);
                        if (deflectCard == null)
                        {
                            messageToLog += $"Player {targetedPlayer.User.Name} drew 3 cards. He had a wild card.";
                            DrawCard(targetedPlayer, 3, false);
                        }
                        else
                        {
                            LastCardPlayed = new LastCardPlayed(pickedCardColor, deflectCard.Value, deflectCard.ImageUrl, targetedPlayer.User.Name);
                            targetedPlayer.Cards.Remove(deflectCard);
                            DiscardedPile.Add(deflectCard);
                            DrawCard(PlayerToPlay, 3, false);
                            messageToLog += $"Player {targetedPlayer.User.Name} has auto deflected judgement card. {player.User.Name} must draw 3 cards.";
                        }
                    }
                    else
                    {
                        messageToLog += $"Player {targetedPlayer.User.Name} didn't draw any cards, he didn't have any wild cards.";
                    }
                    turnResult.MessagesToLog.Add(messageToLog);
                }
                else if (card.Value == CardValue.UnitedWeFall)
                {
                    Players.ForEach(x => DrawCard(x, 2, false));
                    turnResult.MessagesToLog.Add($"Player {player.User.Name} played united we fall card. Every player has drew 2 cards.");
                }
                else if (card.Value == CardValue.ParadigmShift)
                {
                    List<Card> firstCardsBackup = null;
                    for (int i = 0; i < Players.Count; i++)
                    {
                        if (i == 0)
                            firstCardsBackup = Players[i].Cards.ToList();
                        if (i != Players.Count - 1)
                        {
                            Players[i].Cards = Players[i + 1].Cards;
                        }
                        else
                        {
                            Players[i].Cards = firstCardsBackup;
                        }
                    }
                    turnResult.MessagesToLog.Add($"Player {player.User.Name} played paradigm shift card. Every player exchanged their hand with the next player.");
                }
            }
            else
            {
                if (card.Value == CardValue.DrawTwo)
                {
                    var targetedPlayer = GetNextPlayerToPlay();
                    var messageToLog = $"Player {player.User.Name} targeted player {targetedPlayer.User.Name} with +2 card.";
                    var deflectCard = targetedPlayer.Cards.FirstOrDefault(x => x.Value == CardValue.Deflect);
                    if (deflectCard == null)
                    {
                        messageToLog += $"Player {targetedPlayer.User.Name} drew 2 cards.";
                        DrawCard(targetedPlayer, 2, false);
                    }
                    else
                    {
                        LastCardPlayed = new LastCardPlayed(pickedCardColor, deflectCard.Value, deflectCard.ImageUrl, targetedPlayer.User.Name);
                        targetedPlayer.Cards.Remove(deflectCard);
                        DiscardedPile.Add(deflectCard);
                        DrawCard(player, 2, false);
                        messageToLog += $"Player {targetedPlayer.User.Name} has auto deflected +2 card. {player.User.Name} must draw 2 cards.";

                    }
                    turnResult.MessagesToLog.Add(messageToLog);
                }
                else if (card.Value == CardValue.Reverse)
                {
                    turnResult.MessagesToLog.Add($"Player {player.User.Name} has changed direction");
                    Direction = Direction == Direction.Right ? Direction.Left : Direction.Right;
                }
                else if (card.Value == CardValue.Skip)
                {
                    turnResult.MessagesToLog.Add($"Player {player.User.Name} played skip turn. Player {GetNextPlayerToPlay().User.Name} was skipped");
                    PlayerToPlay = GetNextPlayerToPlay();
                }
                else if (card.Value == CardValue.StealTurn)
                {
                    turnResult.MessagesToLog.Add($"Player {player.User.Name} played steal turn. Rotation continues from him.");
                    PlayerToPlay = player;
                }
                else
                {
                    //normal card
                    turnResult.MessagesToLog.Add($"Player {player.User.Name} played card number {card.Value.ToString()} in color {card.Color.ToString()}.");
                }
            }

            UpdateGameAndRoundStatus();
            if (GameEnded)
            {
                turnResult.MessagesToLog.Add("Game has ended");
                return turnResult;
            }
            if (RoundEnded)
            {
                StartGame();
                turnResult.MessagesToLog.Add("Round ended! Starting new round!");
                return turnResult;
            }

            PlayerToPlay = GetNextPlayerToPlay();
            return turnResult;
        }



        public void StartGame()
        {
            Card lastCardDrew;
            Deck = new Deck(GameSetup.GameMode);
            do
            {
                lastCardDrew = Deck.Draw(1).First();
                DiscardedPile.Add(lastCardDrew);
            } while (lastCardDrew.Color == CardColor.Wild);
            LastCardPlayed = new LastCardPlayed(lastCardDrew.Color, lastCardDrew.Value, lastCardDrew.ImageUrl, string.Empty);
            Direction = Direction.Right;
            PlayerToPlay = Players.First();
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
                DiscardedPile.Clear();
                player.Cards.AddRange(Deck.Draw(count - deckCount));
            }
            else
            {
                player.Cards.AddRange(Deck.Draw(count));
            }

            if (normalDraw)
            {
                // if it's normalDraw then it's not a result of a wildcard
                PlayerToPlay = GetNextPlayerToPlay();
            }
        }


        // -------------------------------------private------------


        private Player GetNextPlayerToPlay()
        {
            var indexOfCurrentPlayer = Players.IndexOf(PlayerToPlay);
            if (Direction == Direction.Right)
            {
                if (indexOfCurrentPlayer == Players.Count - 1)
                {
                    return Players.First();
                }
                else
                {
                    return Players[indexOfCurrentPlayer + 1];
                }
            }
            if (Direction == Direction.Left)
            {
                if (indexOfCurrentPlayer == 0)
                {
                    return Players.Last();
                }
                else
                {
                    return Players[indexOfCurrentPlayer - 1];
                }
            }
            throw new Exception("Error, can't access that direction");

        }

        private void UpdateGameAndRoundStatus()
        {
            var playersWithoutCards = Players.Where(x => !x.Cards.Any());
            if (playersWithoutCards.Any())
            {
                foreach (var player in playersWithoutCards)
                {
                    player.RoundsWonCount++;
                }
                RoundEnded = true;
            }
            var playersThatMatchWinCriteria = Players.Where(x => x.RoundsWonCount == GameSetup.RoundsToWin);
            if (playersThatMatchWinCriteria.Any())
            {
                GameEnded = true;
            }
        }
    }
}