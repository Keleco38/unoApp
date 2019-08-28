using System;
using System.Collections.Generic;
using EntityObjects.Cards.Abstraction;

namespace EntityObjects
{
    public class Player
    {
        public Player(User user)
        {
            Id = Guid.NewGuid().ToString();
            User = user;
            Cards = new List<ICard>();
            RoundsWonCount = 0;
        }
        public string Id { get; set; }
        public User User { get; set; }
        public List<ICard> Cards { get; set; }
        public bool LeftGame { get; set; }
        public int RoundsWonCount { get; set; }
        public ICard CardPromisedToDiscard { get; set; }

    }
}