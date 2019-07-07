using System;
using System.Collections.Generic;

namespace Uno.Models
{
    public class GameSetup
    {

        public GameSetup(User user)
        {
            Id = Guid.NewGuid();
            Users = new List<User>() { user };
        }

        public Guid Id { get; set; }
        public List<User> Users { get; set; }
    }
}