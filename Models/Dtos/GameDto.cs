using System.Collections.Generic;
using Uno.Enums;

namespace Uno.Models.Dtos
{
    public class GameDto
    {
        public int DeckSize { get; set; }
        public List<Card> MyCards { get; set; }
        public List<PlayerDto> Players { get; set; }
        public List<User> Spectators { get; set; }
        public List<Card> DiscardedPile { get; set; }
        public GameSetupDto GameSetup { get; set; }
        public Direction Direction { get; set; }
        public Card LastCardPlayed { get; set; }
        public PlayerDto PlayerToPlay { get; set; }
        public bool GameStarted { get; set; }
        public bool GameEnded { get; set; }
    }

}