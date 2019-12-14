using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;
using EntityObjects;

namespace Repository
{
    public class HallOfFameRepository : IHallOfFameRepository
    {
        private const string FolderPath = "hall-of-fame";
        private static readonly string FilePath = Path.Combine(FolderPath, "rankings.csv");

        public void AddPoints(List<string> usernames, int points)
        {
            var existingUsernames = new List<string>();

            var records = LoadAllRankingsFromFile();

            foreach (var record in records)
            {
                if (usernames.Contains(record.Name))
                {
                    existingUsernames.Add(record.Name);
                    record.Points += points;
                    record.GamesWon += 1;
                }
            }

            if (existingUsernames.Count != usernames.Count)
            {
                var usernamesWithoutRecord = usernames.Except(existingUsernames);
                foreach (var username in usernamesWithoutRecord)
                {
                    records.Add(new HallOfFame(username, points, 1));
                }
            }

            WriteRankingsToFile(records);
        }


        public List<HallOfFame> GetTopFiftyPlayers()
        {
            var records = LoadAllRankingsFromFile();
            return records.OrderByDescending(x => x.Points).Take(50).ToList();
        }

        public List<HallOfFame> GetScoresForUsernames(List<string> usernames)
        {
            var records = LoadAllRankingsFromFile();
            return records.Where(x => usernames.Contains(x.Name)).OrderByDescending(x => x.Points).ToList();
        }


        private List<HallOfFame> LoadAllRankingsFromFile()
        {
            Directory.CreateDirectory(FolderPath);

            if (!File.Exists(FilePath))
            {
                return new List<HallOfFame>();
            }

            using (var stream = File.OpenRead(FilePath))
            using (var reader = new StreamReader(stream))
            using (var csv = new CsvReader(reader))
            {
                csv.Configuration.Delimiter = ",";
                csv.Configuration.PrepareHeaderForMatch = (string header, int index) => header.ToLower();
                return csv.GetRecords<HallOfFame>().ToList();
            }
        }


        private void WriteRankingsToFile(List<HallOfFame> records)
        {
            Directory.CreateDirectory(FolderPath);

            using (var stream = File.OpenWrite(FilePath))
            using (var writer = new StreamWriter(stream))
            using (var csv = new CsvWriter(writer))
            {
                csv.WriteRecords(records);
            }
        }
    }
}