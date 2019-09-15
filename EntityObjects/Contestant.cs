namespace EntityObjects
{
    public class Contestant
    {
        public Contestant(User user)
        {
            User = user;
            LeftTournament = false;
        }

        public User User { get; set; }
        public bool LeftTournament { get; set; }
    }
}