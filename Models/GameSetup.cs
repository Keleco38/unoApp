using System;
using System.Collections.Generic;

namespace Uno.Models
{
    public class GameSetup
    {
        public GameSetup()
        {
            Id = Guid.NewGuid().ToString();
        }
        public string Id { get; set; }
        public string Password { get; set; }
        public bool IsPasswordProtected { get => Password.Length > 0; }
    }
}