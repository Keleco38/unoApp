namespace EntityObjects
{
    public class HallOfFame
    {
        public HallOfFame(string name, int points)
        {
            Name = name;
            Points = points;
        }

        public string Name { get; set; }
        public int Points { get; set; }
    }
}