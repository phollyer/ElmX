using ElmX.Commands.Options;
using ElmX.Core.Console;
using System.Text.Json;

namespace ElmX.Core
{
    class Json
    {
        public ElmXJson json = new();

        public readonly bool Exists = false;

        public Json()
        {
            if (File.Exists("elmx.json"))
            {
                Exists = true;
            }

        }

        public void Create(InitOptions options)
        {
            json.EntryFile = options.EntryFile;
            json.ExcludedDirs = options.ExcludedDirs;
            json.ExcludedFiles = options.ExcludedFiles;

            string jsonStr = JsonSerializer.Serialize(json, new JsonSerializerOptions { WriteIndented = true });

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
                    this.json = _json;
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
    class ElmXJson
    {
        public string EntryFile { get; set; } = "";

        public List<string> ExcludedDirs { get; set; } = new List<string>();

        public List<string> ExcludedFiles { get; set; } = new List<string>();
    }
}