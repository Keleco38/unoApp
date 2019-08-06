using System.Collections.Generic;
using Uno.Enums;

namespace Uno.Models.Dtos
{
    public class GameDto
    {
        public DeckDto Deck { get; set; }
        public List<PlayerDto> Players { get; set; }
        public List<SpectatorDto> Spectators { get; set; }
        public GameSetupDto GameSetup { get; set; }
        public Direction Direction { get; set; }
        public LastCardPlayedDto LastCardPlayed { get; set; }
        public PlayerDto PlayerToPlay { get; set; }
        public bool GameStarted { get; set; }
        public List<CardDto> DiscardedPile { get; set; }
        public bool GameEnded { get; set; }
    }

}