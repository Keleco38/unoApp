namespace Uno.Models
{
    public class User
    {
        public User(string connectionId, string name)
        {
            ConnectionId = connectionId;
            Name = name;
        }
        public string Name { get; set; }
        public string ConnectionId { get; set; }
    }
}