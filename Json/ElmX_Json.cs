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
        public readonly Json? Json;

        private readonly bool Exists = false;

        public ElmX_Json()
        {
            if (File.Exists("elmx.json"))
            {
                Exists = true;
                Json Json = Read();
            }

        }

        public void Create(string entryFile, List<string> excludedDirs, List<string> excludedFiles)
        {
            if (Exists && Json != null)
            {
                PrintExistingValues(Json);

                Writer.Write("Do you want to overwrite the elmx.json file? (y/n) ");

                string key = Reader.ReadKey();

                if (key == "y")
                {
                    File.Delete("elmx.json");

                    Json.EntryFile = entryFile;
                    Json.ExcludedDirs = excludedDirs;
                    Json.ExcludedFiles = excludedFiles;

                    string jsonStr = JsonSerializer.Serialize(Json, new JsonSerializerOptions { WriteIndented = true });

                    File.WriteAllText("elmx.json", jsonStr);

                    Writer.EmptyLine();
                }
                else
                {
                    Writer.EmptyLine();
                    Writer.WriteLine("Exiting...");
                    Environment.Exit(0);
                }
            }
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
                    json = _json;
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

        private void PrintExistingValues(Json json)
        {
            Writer.WriteLine("The elmx.json file already exists with following values:");
            Writer.EmptyLine();
            Writer.WriteLine($"Entry File: {json.EntryFile}");
            Writer.EmptyLine();
            Writer.WriteLine("Excluded Directories:");
            Writer.EmptyLine();
            Writer.WriteLines(json.ExcludedDirs);
            Writer.EmptyLine();
            Writer.WriteLine("Excluded Files:");
            Writer.EmptyLine();
            Writer.WriteLines(json.ExcludedFiles);
            Writer.EmptyLine();
        }
    }
}