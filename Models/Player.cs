using System.Collections.Generic;
using Uno.Enums;

namespace Uno.Models
{
    public class Player
    {
        public Player(User user, List<Card> hand)
        {
            User = user;
            Hand = hand;
        }
        public User User { get; set; }
        public List<Card> Hand { get; set; }
        public bool LeftGame { get; set; }

    }
}