using System;

namespace Uno.Models
{
    public class User
    {
        public User(string connectionId, string name)
        {
            ConnectionId = connectionId;
            Name = name;
            LastBuzzedUtc=DateTime.Now;
        }
        public string Name { get; set; }
        public string ConnectionId { get; set; }
        public DateTime LastBuzzedUtc { get; set; }

    }
}