// A class that  reads an elm.json file.

using ElmX.Console;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ElmX.Json
{
    class ElmJson
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = "";

        [JsonPropertyName("source-directories")]
        public List<string> SourceDirs { get; set; } = new List<string>();

        [JsonPropertyName("elm-version")]
        public string ElmVersion { get; set; } = "";

        [JsonPropertyName("dependencies")]
        public Dependencies Dependencies { get; set; } = new Dependencies();

        [JsonPropertyName("test-dependencies")]
        public Dependencies TestDependencies { get; set; } = new Dependencies();
    }
    class Dependencies
    {
        [JsonPropertyName("direct")]
        public Dictionary<string, string> Direct { get; set; } = new Dictionary<string, string>();

        [JsonPropertyName("indirect")]
        public Dictionary<string, string> Indirect { get; set; } = new Dictionary<string, string>();
    }
    class Elm_Json
    {
        public ElmJson Json = new();

        public readonly bool Exists = false;
        public Elm_Json()
        {
            if (File.Exists("elm.json"))
            {
                Exists = true;
            }
        }

        public void Read()
        {
            if (File.Exists("elm.json"))
            {
                string jsonStr = File.ReadAllText("elm.json");

                try
                {
                    ElmJson? json = JsonSerializer.Deserialize<ElmJson>(jsonStr);

                    if (json != null)
                    {
                        Json = json;
                    }
                }
                catch (Exception e)
                {
                    Writer.EmptyLine();
                    Writer.WriteLine("There was an error reading the elm.json file.");
                    Writer.WriteLine(e.Message);
                    Writer.EmptyLine();
                    Writer.WriteLine("Exiting...");
                    Environment.Exit(1);
                }
            }
        }
    }
}