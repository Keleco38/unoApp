using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;
using EntityObjects;

namespace Repository
{
    public class StickyTournamentRepository : IStickyTournamentRepository
    {
        private const string FolderPath = "sticky-tournaments";
        private static readonly string FilePath = Path.Combine(FolderPath, "records.csv");

        public void AddOrUpdateStickyTournament(string name, string url)
        {
            var records = LoadAllStickyTournaments();

            var selectedTournament = records.FirstOrDefault(x => x.Name == name);

            if (selectedTournament == null)
            {
                records.Add(new StickyTournament(name, url, false));
            }
            else
            {
                selectedTournament.Name = name;
                selectedTournament.Url = url;
                selectedTournament.Disabled = false;
            }

            WriteStickyTournamentToFile(records);
        }

        public void DeleteStickyTournament(string name)
        {
            var records = LoadAllStickyTournaments();

            var selectedTournament = records.FirstOrDefault(x => x.Name == name);

            if (selectedTournament == null)
                return;

            selectedTournament.Disabled = true;
            WriteStickyTournamentToFile(records.ToList());
        }



        public List<StickyTournament> GetAllStickyTournaments()
        {
            var records = LoadAllStickyTournaments();
            return records.Where(x => !x.Disabled).ToList();
        }


        private List<StickyTournament> LoadAllStickyTournaments()
        {
            Directory.CreateDirectory(FolderPath);

            if (!File.Exists(FilePath))
            {
                return new List<StickyTournament>();
            }

            using (var stream = File.OpenRead(FilePath))
            using (var reader = new StreamReader(stream))
            using (var csv = new CsvReader(reader))
            {
                csv.Configuration.Delimiter = ",";
                csv.Configuration.PrepareHeaderForMatch = (string header, int index) => header.ToLower();
                return csv.GetRecords<StickyTournament>().ToList();
            }
        }


        private void WriteStickyTournamentToFile(List<StickyTournament> records)
        {
            using (var stream = File.OpenWrite(FilePath))
            using (var writer = new StreamWriter(stream))
            using (var csv = new CsvWriter(writer))
            {
                csv.WriteRecords(records);
            }
        }
    }
}