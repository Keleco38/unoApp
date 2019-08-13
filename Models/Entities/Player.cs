using System;
using System.Collections.Generic;
using Uno.Models.Entities.Cards.Abstraction;

namespace Uno.Models.Entities
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

    }
}