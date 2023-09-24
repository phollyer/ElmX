using ElmX.Core.Console;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ElmX.Core
{
    public class Json
    {
        public ElmXJson json = new();

        public readonly bool Exists = false;

        public Json()
        {
            Exists = File.Exists("elmx.json");
        }

        public void Create(Commands.Init.Options options)
        {
            json.EntryFile = options.EntryFile;
            json.ExcludeDirs = options.ExcludeDirs;
            json.ExcludeFiles = options.ExcludeFiles;

            string jsonStr = JsonSerializer.Serialize(json, new JsonSerializerOptions { WriteIndented = true });

            File.WriteAllText("elmx.json", jsonStr);
        }

        public void Read()
        {
            string jsonStr = File.ReadAllText("elmx.json");

            try
            {
                ElmXJson? _json = JsonSerializer.Deserialize<ElmXJson>(jsonStr);

                if (_json != null)
                {
                    json = _json;
                }
                else
                {
                    Writer.WriteLine("There was an error reading the elmx.json file.");
                    Environment.Exit(1);
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
        }
    }
    public class ElmXJson
    {
        [JsonPropertyName("entry-file")]
        public string EntryFile { get; set; } = "";

        [JsonPropertyName("exclude-dirs")]
        public List<string> ExcludeDirs { get; set; } = new List<string>();

        [JsonPropertyName("exclude-files")]
        public List<string> ExcludeFiles { get; set; } = new List<string>();
    }
}