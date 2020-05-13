using System;
using System.Collections.Generic;
using Common.Enums;

namespace DomainObjects
{
    public class GameDto
    {
        public string Id { get; set; }
        public DeckDto Deck { get; set; }
        public List<PlayerDto> Players { get; set; }
        public List<SpectatorDto> Spectators { get; set; }
        public GameSetupDto GameSetup { get; set; }
        public Direction Direction { get; set; }
        public LastCardPlayedDto LastCardPlayed { get; set; }
        public PlayerDto PlayerToPlay { get; set; }
        public bool GameStarted { get; set; }
        public List<string> ReadyPlayersLeft { get; set; }
        public List<CardDto> DiscardedPile { get; set; }
        public bool GameEnded { get; set; }
        public bool IsTournamentGame { get; set; }
        public bool DrawAutoPlay { get; set; }
        public List<UserDto> BannedUsers { get; set; }
        public PlayerDto DrawAutoPlayPlayer { get; set; }
        public CardDto DrawAutoPlayCard { get; set; }
    }

}