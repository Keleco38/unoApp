namespace EntityObjects
{
    public class Spectator
    {
        public Spectator(User user)
        {
            User = user;
        }
        public User User { get; set; }
    }
}