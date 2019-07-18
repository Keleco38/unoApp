using System.Collections.Generic;
using Uno.Enums;

namespace Uno.Models
{
    public class Player
    {
        public Player(User user)
        {
            User = user;
            Cards = new List<Card>();
            RoundsWonCount = 0;
        }
        public User User { get; set; }
        public List<Card> Cards { get; set; }
        public bool LeftGame { get; set; }
        public int RoundsWonCount { get; set; }

    }
}