using System;

namespace EntityObjects
{
    public class User
    {
        public User(string connectionId, string name, string ipHash)
        {
            ConnectionId = connectionId;
            Name = name;
            LastBuzzedUtc=DateTime.Now;
            IPHash = ipHash;
        }
        public string Name { get; set; }
        public string ConnectionId { get; set; }
        public string ActiveGameId { get; set; }
        public string IPHash { get; set; }
        public string ActiveTournamentId { get; set; }
        public DateTime LastBuzzedUtc { get; set; }

    }
}