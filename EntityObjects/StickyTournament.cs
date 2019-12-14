namespace EntityObjects
{
    public class StickyTournament
    {
        public StickyTournament(string name, string url, bool disabled)
        {
            Name = name;
            Url = url;
            Disabled = disabled;
        }

        public string Name { get; set; }
        public string Url { get; set; }
        public bool Disabled { get; set; }
    }
}