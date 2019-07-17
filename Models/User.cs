using System;

namespace Uno.Models
{
    public class User
    {
        public User(string connectionId, string name)
        {
            ConnectionId = connectionId;
            Name = name;
            LastTimeBuzzed=DateTime.Now;
        }
        public string Name { get; set; }
        public string ConnectionId { get; set; }
        public DateTime LastTimeBuzzed { get; set; }

    }
}