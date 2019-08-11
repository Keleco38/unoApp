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
        }

        public TurnResult PlayCard(Player player, Card cardPlayed, CardColor pickedCardColor, string targetedPlayerName, Card cardToDig, List<int> duelNumbers, List<Card> charityCards, int blackjackNumber, List<int> numbersToDiscard)
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

            LastCardPlayed = new LastCardPlayed(pickedCardColor, card.Value, card.ImageUrl, player.User.Name, cardPlayed.Color == CardColor.Wild);


            if (!string.IsNullOrEmpty(targetedPlayerName))
            {
                Players.Where(p => p != player).ToList().ForEach(x =>
                        {
                            var magneticCard = x.Cards.FirstOrDefault(c => c.Value == CardValue.MagneticPolarity);
                            if (magneticCard != null)
                            {
                                LastCardPlayed = new LastCardPlayed(pickedCardColor, magneticCard.Value, magneticCard.ImageUrl, x.User.Name, true);
                                x.Cards.Remove(magneticCard);
                                DiscardedPile.Add(magneticCard);
                                turnResult.MessagesToLog.Add($"Player {x.User.Name} activated magnetic polarity. He was the target instead of {targetedPlayerName}.");
                                targetedPlayerName = x.User.Name;
                            }
                        });
            }


            if (card.Color == CardColor.Wild)
            {
                if (card.Value == CardValue.ChangeColor)
                {
                    turnResult.MessagesToLog.Add($"Player {player.User.Name} changed color to {pickedCardColor}.");
                }
                if (card.Value == CardValue.MagneticPolarity)
                {
                    turnResult.MessagesToLog.Add($"Player {player.User.Name} changed color to {pickedCardColor} (magneticpolarity).");
                }
                if (card.Value == CardValue.Deflect)
                {
                    turnResult.MessagesToLog.Add($"Player {player.User.Name} changed color to {pickedCardColor} (deflect card).");
                }
                if (card.Value == CardValue.KeepMyHand)
                {
                    turnResult.MessagesToLog.Add($"Player {player.User.Name} changed color to {pickedCardColor} (keep my hand card).");
                }
                if (card.Value == CardValue.TheLastStand)
                {
                    turnResult.MessagesToLog.Add($"Player {player.User.Name} changed color to {pickedCardColor} (keep my hand card).");
                }
                else if (card.Value == CardValue.DrawFour)
                {
                    var targetedPlayer = GetNextPlayer(PlayerToPlay, Players);
                    var messageToLog = $"Player {player.User.Name} targeted player {targetedPlayer.User.Name} with +4 card.";
                    var deflectCard = targetedPlayer.Cards.FirstOrDefault(x => x.Value == CardValue.Deflect);
                    if (deflectCard == null)
                    {
                        messageToLog += $"Player {targetedPlayer.User.Name} drew 4 cards.";
                        DrawCard(targetedPlayer, 4, false);
                    }
                    else
                    {
                        LastCardPlayed = new LastCardPlayed(pickedCardColor, deflectCard.Value, deflectCard.ImageUrl, targetedPlayer.User.Name, true);
                        targetedPlayer.Cards.Remove(deflectCard);
                        DiscardedPile.Add(deflectCard);
                        DrawCard(player, 4, false);
                        messageToLog += $"Player {targetedPlayer.User.Name} auto deflected +4 card. {player.User.Name} must draw 4 cards.";

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
                    var messageToLog = $"{player.User.Name} targeted {targetedPlayer.User.Name} with swap hands card. ";

                    var keepMyHandCard = targetedPlayer.Cards.FirstOrDefault(y => y.Value == CardValue.KeepMyHand);
                    if (keepMyHandCard != null)
                    {
                        LastCardPlayed = new LastCardPlayed(pickedCardColor, keepMyHandCard.Value, keepMyHandCard.ImageUrl, PlayerToPlay.User.Name, true);
                        targetedPlayer.Cards.Remove(keepMyHandCard);
                        DiscardedPile.Add(keepMyHandCard);
                        messageToLog += $"{targetedPlayer.User.Name} kept his hand safe. ";
                    }
                    else
                    {
                        var playersCards = PlayerToPlay.Cards.ToList();
                        var targetedPlayerCards = targetedPlayer.Cards.ToList();

                        PlayerToPlay.Cards = targetedPlayerCards;
                        targetedPlayer.Cards = playersCards;
                        messageToLog += $"Players exchanged hands.";
                    }

                    turnResult.MessagesToLog.Add(messageToLog);
                }
                else if (card.Value == CardValue.BlackHole)
                {
                    var messageToLog = $"{player.User.Name} played black hole card. Every player drew new hand. ";
                    Players.ForEach(x =>
                    {
                        var keepMyHandCard = x.Cards.FirstOrDefault(y => y.Value == CardValue.KeepMyHand);
                        if (keepMyHandCard != null)
                        {
                            LastCardPlayed = new LastCardPlayed(pickedCardColor, keepMyHandCard.Value, keepMyHandCard.ImageUrl, PlayerToPlay.User.Name, true);
                            x.Cards.Remove(keepMyHandCard);
                            DiscardedPile.Add(keepMyHandCard);
                            messageToLog += $"{x.User.Name} kept his hand safe. ";
                        }
                        else
                        {
                            var cardCount = x.Cards.Count;
                            DiscardedPile.AddRange(x.Cards.ToList());
                            x.Cards.Clear();
                            DrawCard(x, cardCount, false);
                        }

                    });
                    turnResult.MessagesToLog.Add(messageToLog);
                }
                else if (card.Value == CardValue.DoubleEdge)
                {
                    var targetedPlayer = Players.Find(x => x.User.Name == targetedPlayerName);

                    var messageToLog = $"{player.User.Name} targeted {targetedPlayer.User.Name} with double edge card. ";

                    var deflectCard = targetedPlayer.Cards.FirstOrDefault(x => x.Value == CardValue.Deflect);
                    if (deflectCard == null)
                    {
                        messageToLog += $"{targetedPlayer.User.Name} drew 5 cards. ";
                        messageToLog += $"{player.User.Name} drew 2 cards. ";
                        DrawCard(targetedPlayer, 5, false);
                        DrawCard(PlayerToPlay, 2, false);
                    }
                    else
                    {
                        LastCardPlayed = new LastCardPlayed(pickedCardColor, deflectCard.Value, deflectCard.ImageUrl, targetedPlayer.User.Name, true);
                        targetedPlayer.Cards.Remove(deflectCard);
                        DiscardedPile.Add(deflectCard);
                        DrawCard(PlayerToPlay, 5, false);
                        DrawCard(targetedPlayer, 2, false);
                        messageToLog += $"{targetedPlayer.User.Name} auto deflected double edge card. He will draw 2 cards and {player.User.Name} must draw 5 cards.";
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
                    var colorIds = new int[] { 1, 2, 3, 4 };
                    int randomColor = colorIds[(random.Next(4))];
                    LastCardPlayed.Color = (CardColor)randomColor;
                    turnResult.MessagesToLog.Add($"{player.User.Name} played discard color card. All players discarded {pickedCardColor} and a random color has been assigned.");

                }
                else if (card.Value == CardValue.HandOfGod)
                {
                    if (player.Cards.Count > 7)
                    {
                        turnResult.MessagesToLog.Add($"{player.User.Name} discarded 4 cards (hand of god). ");
                        var cards = player.Cards.Take(4).ToList();
                        DiscardedPile.AddRange(cards);
                        cards.ForEach(y => player.Cards.Remove(y));
                    }
                    else
                    {
                        turnResult.MessagesToLog.Add($"{player.User.Name} didn't discard any cards. He had less than 8 cards. (hand of god)");
                    }
                }
                else if (card.Value == CardValue.Judgement)
                {
                    var targetedPlayer = Players.Find(x => x.User.Name == targetedPlayerName);
                    var messageToLog = $"{player.User.Name} targeted {targetedPlayer.User.Name} with the judgement card. ";
                    if (targetedPlayer.Cards.Any(x => x.Color == CardColor.Wild))
                    {
                        var deflectCard = targetedPlayer.Cards.FirstOrDefault(x => x.Value == CardValue.Deflect);
                        if (deflectCard == null)
                        {
                            messageToLog += $"{targetedPlayer.User.Name} drew 3 cards. He had a wild card.";
                            DrawCard(targetedPlayer, 3, false);
                        }
                        else
                        {
                            LastCardPlayed = new LastCardPlayed(pickedCardColor, deflectCard.Value, deflectCard.ImageUrl, targetedPlayer.User.Name, true);
                            targetedPlayer.Cards.Remove(deflectCard);
                            DiscardedPile.Add(deflectCard);
                            DrawCard(PlayerToPlay, 3, false);
                            messageToLog += $"{targetedPlayer.User.Name} auto deflected judgement card. {player.User.Name} must draw 3 cards.";
                        }
                    }
                    else
                    {
                        messageToLog += $"{targetedPlayer.User.Name} didn't draw any cards, he didn't have any wild cards.";
                    }
                    turnResult.MessagesToLog.Add(messageToLog);
                }
                else if (card.Value == CardValue.UnitedWeFall)
                {
                    Players.ForEach(x => DrawCard(x, 2, false));
                    turnResult.MessagesToLog.Add($"{player.User.Name} played united we fall card. Every player drew 2 cards.");
                }
                else if (card.Value == CardValue.ParadigmShift)
                {
                    var messageToLog = $"{player.User.Name} played paradigm shift card. Every player exchanged their hand with the next player. ";
                    List<Card> firstCardsBackup = null;

                    var playersWithKeepMyHandCard = Players.Where(x => x.Cards.FirstOrDefault(y => y.Value == CardValue.KeepMyHand) != null).ToList();
                    var playersWithOutKeepMyHandCard = Players.Where(x => x.Cards.FirstOrDefault(y => y.Value == CardValue.KeepMyHand) == null).ToList();

                    playersWithKeepMyHandCard.ForEach(x =>
                    {
                        var keepMyHandCard = x.Cards.FirstOrDefault(y => y.Value == CardValue.KeepMyHand);
                        LastCardPlayed = new LastCardPlayed(pickedCardColor, keepMyHandCard.Value, keepMyHandCard.ImageUrl, PlayerToPlay.User.Name, true);
                        x.Cards.Remove(keepMyHandCard);
                        DiscardedPile.Add(keepMyHandCard);
                        messageToLog += $"{x.User.Name} kept his hand safe. ";
                    });

                    Player loopingPlayer = null;

                    for (int i = 0; i < playersWithOutKeepMyHandCard.Count; i++)
                    {
                        if (i == 0)
                        {
                            loopingPlayer = playersWithOutKeepMyHandCard[0];
                            firstCardsBackup = loopingPlayer.Cards.ToList();
                        }

                        if (i != playersWithOutKeepMyHandCard.Count - 1)
                        {
                            loopingPlayer.Cards = GetNextPlayer(loopingPlayer, playersWithOutKeepMyHandCard).Cards;
                        }
                        else
                        {
                            loopingPlayer.Cards = firstCardsBackup;
                        }
                        loopingPlayer = GetNextPlayer(loopingPlayer, playersWithOutKeepMyHandCard);
                    }

                    turnResult.MessagesToLog.Add(messageToLog);
                }
                else if (card.Value == CardValue.GraveDigger)
                {
                    player.Cards.Add(cardToDig);
                    DiscardedPile.Remove(cardToDig);
                    turnResult.MessagesToLog.Add($"{player.User.Name} digged card {cardToDig.Color.ToString()} {cardToDig.Value.ToString()} from the discarded pile.");
                }
                else if (card.Value == CardValue.RussianRoulette)
                {
                    var messageToLog = $"{player.User.Name} played Russian Roulette. Every player rolled a dice.";
                    var playerRolling = GetNextPlayer(PlayerToPlay, Players);
                    Random random = new Random();

                    while (true)
                    {
                        int rolledNumber = random.Next(1, 7);
                        messageToLog += $" [{playerRolling.User.Name}: {rolledNumber}] ";
                        if (rolledNumber == 1)
                        {
                            messageToLog += $" {playerRolling.User.Name} drew 6 cards.";
                            DrawCard(playerRolling, 6, false);
                            break;
                        }
                        playerRolling = GetNextPlayer(playerRolling, Players);
                    }
                    turnResult.MessagesToLog.Add(messageToLog);
                }
                else if (card.Value == CardValue.Roulette)
                {
                    Random random = new Random();
                    var messageToLog = $"{player.User.Name} played Roulette. ";
                    var drawOrDiscard = random.Next(2);
                    var playerAffected = Players[random.Next(Players.Count)];
                    if (drawOrDiscard == 0)
                    {
                        //discard   
                        var numberOfCardsToDiscard = random.Next(1, 3);
                        messageToLog += $"{playerAffected.User.Name} is a lucky winner! He will discard {numberOfCardsToDiscard} cards.";
                        playerAffected.Cards.RemoveRange(0, numberOfCardsToDiscard);
                    }
                    else
                    {
                        //draw
                        var numberOfCardsToDraw = random.Next(1, 5);
                        messageToLog += $"{playerAffected.User.Name} didn't have any luck! He will draw {numberOfCardsToDraw} cards.";
                        DrawCard(playerAffected, numberOfCardsToDraw, false);
                    }

                    turnResult.MessagesToLog.Add(messageToLog);
                }
                else if (card.Value == CardValue.Duel)
                {
                    Random random = new Random();
                    var numberRolled = random.Next(1, 7);
                    var targetedPlayer = Players.Find(x => x.User.Name == targetedPlayerName);
                    var maxNumberCalledPicked = duelNumbers.Max();
                    var callerWon = duelNumbers.Contains(numberRolled);
                    var messageToLog = $"{player.User.Name} targeted {targetedPlayer.User.Name} with card Duel. Numbers he picked: {String.Join(' ', duelNumbers)}. Number rolled: {numberRolled}. ";
                    if (callerWon)
                    {
                        DrawCard(targetedPlayer, maxNumberCalledPicked, false);
                        messageToLog += $"{PlayerToPlay.User.Name} won! {targetedPlayer.User.Name} will draw {maxNumberCalledPicked} cards (max number player selected).";
                    }
                    else
                    {
                        DrawCard(PlayerToPlay, maxNumberCalledPicked, false);
                        messageToLog += $"{targetedPlayer.User.Name} won! {PlayerToPlay.User.Name} will draw {maxNumberCalledPicked} (max number player selected) cards.";
                    }
                    turnResult.MessagesToLog.Add(messageToLog);
                }
                else if (card.Value == CardValue.FairPlay)
                {
                    var targetedPlayer = Players.Find(x => x.User.Name == targetedPlayerName);
                    var messageToLog = $"{player.User.Name} targeted {targetedPlayer.User.Name} with card Fair Play. ";

                    var cardDifference = targetedPlayer.Cards.Count - PlayerToPlay.Cards.Count;

                    if (cardDifference == 0)
                    {
                        messageToLog += "No effect. They had the same numer of cards.";
                    }
                    else if (cardDifference > 0)
                    {
                        //targeted Player discards
                        targetedPlayer.Cards.RemoveRange(0, cardDifference);
                        messageToLog += $"{targetedPlayer.User.Name} discarded {cardDifference} cards. He had more cards.";
                    }
                    else
                    {
                        //targeted Player draws
                        var numberInPositiveValue = cardDifference * -1;
                        DrawCard(targetedPlayer, numberInPositiveValue, false);
                        messageToLog += $"{targetedPlayer.User.Name} must draw {numberInPositiveValue} cards. He had less cards.";
                    }

                    turnResult.MessagesToLog.Add(messageToLog);
                }
                else if (card.Value == CardValue.Charity)
                {
                    var charityCardsString = string.Empty;
                    var targetedPlayer = Players.Find(x => x.User.Name == targetedPlayerName);
                    charityCards.ForEach(x =>
                    {
                        charityCardsString += x.Color + " " + x.Value + ", ";
                        player.Cards.Remove(player.Cards.First(c => c.Value == x.Value && c.Color == x.Color));
                        targetedPlayer.Cards.Add(x);
                    });

                    var messageToLog = $"{player.User.Name} targeted {targetedPlayer.User.Name} with card Charity. He gave him two cards: {charityCardsString}";
                    turnResult.MessagesToLog.Add(messageToLog);
                }
                else if (card.Value == CardValue.TricksOfTheTrade)
                {
                    Random random = new Random();
                    var targetedPlayer = Players.Find(x => x.User.Name == targetedPlayerName);
                    var messageToLog = $"{player.User.Name} targeted {targetedPlayer.User.Name} with card Tricks of the trade. ";
                    var callerNumberToTrade = random.Next(1, player.Cards.Count < 4 ? player.Cards.Count : 4);
                    var targetNumberToTrade = random.Next(1, targetedPlayer.Cards.Count < 4 ? targetedPlayer.Cards.Count : 4);
                    var cardsCallerTraded = player.Cards.GetRange(0, callerNumberToTrade);
                    var cardsTargetTraded = targetedPlayer.Cards.GetRange(0, targetNumberToTrade);
                    var cardsCallerTradedString = string.Empty;
                    cardsCallerTraded.ForEach(x => { cardsCallerTradedString += (x.Color + " " + x.Value + ", "); });
                    messageToLog += $"{player.User.Name} gave {callerNumberToTrade} cards: {cardsCallerTradedString}. ";
                    var cardsTargetTradedString = string.Empty;
                    cardsTargetTraded.ForEach(x => { cardsTargetTradedString += (x.Color + " " + x.Value + ", "); });
                    messageToLog += $"{targetedPlayer.User.Name} gave {targetNumberToTrade} cards: {cardsTargetTradedString}. ";
                    player.Cards.AddRange(cardsTargetTraded);
                    targetedPlayer.Cards.AddRange(cardsCallerTraded);
                    cardsCallerTraded.ForEach(x => player.Cards.Remove(x));
                    cardsTargetTraded.ForEach(x => targetedPlayer.Cards.Remove(x));
                    turnResult.MessagesToLog.Add(messageToLog);
                }
                else if (card.Value == CardValue.Blackjack)
                {
                    var messageToLog = $"{player.User.Name} played blackjack. He hit {blackjackNumber}. ";
                    if (blackjackNumber > 21)
                    {
                        DrawCard(player, 7, false);
                        messageToLog += $"He went over 21. He will draw 7 cards.";
                    }
                    else if (blackjackNumber == 21)
                    {
                        player.Cards.RemoveRange(0, 5);
                        messageToLog += $"He hit the blackjack. He will discard 5 cards.";
                    }
                    else if (blackjackNumber < 21 && blackjackNumber > 17)
                    {
                        player.Cards.RemoveRange(0, 2);
                        messageToLog += $"He beat the dealer. He will discard 2 cards.";
                    }
                    else if (blackjackNumber == 17)
                    {
                        messageToLog += $"It's a draw. Nothing happens. ";
                    }
                    else
                    {
                        var difference = 17 - blackjackNumber;
                        DrawCard(player, difference, false);
                        messageToLog += $"Player pulled out. He will draw {difference} cards.";
                    }
                    turnResult.MessagesToLog.Add(messageToLog);
                }
                else if (card.Value == CardValue.DiscardNumber)
                {
                    var messageToLog = $"{player.User.Name} played discard number. Numbers that are discarded: {string.Join(' ', numbersToDiscard)}. ";
                    Players.ForEach(p =>
                    {
                        var cardsToDiscard = p.Cards.Where(c => numbersToDiscard.Contains((int)c.Value)).ToList();
                        cardsToDiscard.ForEach(x => p.Cards.Remove(x));
                    });
                    turnResult.MessagesToLog.Add(messageToLog);
                }
            }
            else
            {
                if (card.Value == CardValue.DrawTwo)
                {
                    var targetedPlayer = GetNextPlayer(PlayerToPlay, Players);
                    var messageToLog = $"{player.User.Name} targeted {targetedPlayer.User.Name} with +2 card. ";
                    var deflectCard = targetedPlayer.Cards.FirstOrDefault(x => x.Value == CardValue.Deflect);
                    if (deflectCard == null)
                    {
                        messageToLog += $"{targetedPlayer.User.Name} drew 2 cards.";
                        DrawCard(targetedPlayer, 2, false);
                    }
                    else
                    {
                        LastCardPlayed = new LastCardPlayed(pickedCardColor, deflectCard.Value, deflectCard.ImageUrl, targetedPlayer.User.Name, true);
                        targetedPlayer.Cards.Remove(deflectCard);
                        DiscardedPile.Add(deflectCard);
                        DrawCard(player, 2, false);
                        messageToLog += $"{targetedPlayer.User.Name} auto deflected +2 card. {player.User.Name} must draw 2 cards.";

                    }
                    turnResult.MessagesToLog.Add(messageToLog);
                }
                else if (card.Value == CardValue.Reverse)
                {
                    turnResult.MessagesToLog.Add($"{player.User.Name} changed direction");
                    Direction = Direction == Direction.Right ? Direction.Left : Direction.Right;
                }
                else if (card.Value == CardValue.Skip)
                {
                    turnResult.MessagesToLog.Add($"{player.User.Name} played skip turn. {GetNextPlayer(PlayerToPlay, Players).User.Name} was skipped");
                    PlayerToPlay = GetNextPlayer(PlayerToPlay, Players);
                }
                else if (card.Value == CardValue.StealTurn)
                {
                    turnResult.MessagesToLog.Add($"{player.User.Name} played steal turn. Rotation continues from him.");
                    PlayerToPlay = player;
                }
                else
                {
                    //normal card
                    turnResult.MessagesToLog.Add($"{player.User.Name} played card {card.Color.ToString()} {card.Value.ToString()}.");
                }
            }


            UpdateGameAndRoundStatus(turnResult);
            if (GameEnded)
            {
                return turnResult;
            }
            if (RoundEnded)
            {
                StartNewGame();
                return turnResult;
            }

            PlayerToPlay = GetNextPlayer(PlayerToPlay, Players);
            return turnResult;
        }



        public void StartNewGame()
        {
            Random random = new Random();
            Card lastCardDrew;
            DiscardedPile = new List<Card>();
            Deck = new Deck(GameSetup.GameMode);
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


        // -------------------------------------private------------


        private Player GetNextPlayer(Player player, List<Player> ListOfPlayers)
        {
            var indexOfCurrentPlayer = ListOfPlayers.IndexOf(player);
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

        private void UpdateGameAndRoundStatus(TurnResult turnResult)
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
                    turnResult.MessagesToLog.Add($"{firstPlayerWithTheLastStand.User.Name} saved the day! He played The Last Stand. Every player that had 0 cards will draw 2 cards.");
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
                turnResult.MessagesToLog.Add($"Round ended! Players that won that round: {string.Join(',', playersWithoutCards.Select(x => x.User.Name))}");
            }
            var playersThatMatchWinCriteria = Players.Where(x => x.RoundsWonCount == GameSetup.RoundsToWin);
            if (playersThatMatchWinCriteria.Any())
            {
                GameEnded = true;
                turnResult.MessagesToLog.Add($"Game ended! Players that won the game: {string.Join(',', playersThatMatchWinCriteria.Select(x => x.User.Name))}");
            }
        }
    }
}