// A class that reads and writes the ElmX options to a JSON file.

using ElmX.Commands.Options;
using ElmX.Console;
using System.Text.Json;

namespace ElmX.Json
{

    class ElmXJson
    {
        public string EntryFile { get; set; } = "";

        public List<string> ExcludedDirs { get; set; } = new List<string>();

        public List<string> ExcludedFiles { get; set; } = new List<string>();
    }
    class ElmX_Json
    {
        public ElmXJson Json = new();

        public readonly bool Exists = false;

        public ElmX_Json()
        {
            if (File.Exists("elmx.json"))
            {
                Exists = true;
            }

        }

        public void Create(InitOptions options)
        {
            Json.EntryFile = options.EntryFile;
            Json.ExcludedDirs = options.ExcludedDirs;
            Json.ExcludedFiles = options.ExcludedFiles;

            string jsonStr = JsonSerializer.Serialize(Json, new JsonSerializerOptions { WriteIndented = true });

            File.WriteAllText("elmx.json", jsonStr);
        }

        public ElmXJson Read()
        {
            string jsonStr = File.ReadAllText("elmx.json");

            ElmXJson json = new();

            try
            {
                ElmXJson? _json = JsonSerializer.Deserialize<ElmXJson>(jsonStr);

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