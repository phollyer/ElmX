// A class that reads and writes the ElmX options to a JSON file.

using ElmX.Console;
using System.Text.Json;

namespace ElmX.Json
{

    class Json
    {
        public string EntryFile { get; set; } = "";

        public List<string> ExcludedDirs { get; set; } = new List<string>();

        public List<string> ExcludedFiles { get; set; } = new List<string>();
    }
    class ElmX_Json
    {
        public Json Json = new();

        public readonly bool Exists = false;

        public ElmX_Json()
        {
            if (File.Exists("elmx.json"))
            {
                Exists = true;
            }

        }

        public void Create(string entryFile, List<string> excludedDirs, List<string> excludedFiles)
        {
            Json.EntryFile = entryFile;
            Json.ExcludedDirs = excludedDirs;
            Json.ExcludedFiles = excludedFiles;

            string jsonStr = JsonSerializer.Serialize(Json, new JsonSerializerOptions { WriteIndented = true });

            File.WriteAllText("elmx.json", jsonStr);
        }

        public Json Read()
        {
            string jsonStr = File.ReadAllText("elmx.json");

            Json json = new();

            try
            {
                Json? _json = JsonSerializer.Deserialize<Json>(jsonStr);

                if (_json != null)
                {
                    Json = _json;
                }
            }
            catch (Exception e)
            {
                Writer.WriteLine("There was an error reading the elmx.json file.");
                Writer.EmptyLine();
                Writer.WriteLine(e.Message);
                Writer.EmptyLine();
                Writer.WriteLine("Exiting...");
                Environment.Exit(1);
            }

            return json;
        }
    }
}