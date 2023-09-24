using System.Text.Json.Serialization;

namespace ElmX.Elm.App
{
    public class Json
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = "";

        [JsonPropertyName("elm-version")]
        public string ElmVersion { get; set; } = "";

        [JsonPropertyName("source-directories")]
        public List<string> SourceDirs { get; set; } = new List<string>();

        [JsonPropertyName("dependencies")]
        public Dependencies Dependencies { get; set; } = new Dependencies();

        [JsonPropertyName("test-dependencies")]
        public Dependencies TestDependencies { get; set; } = new Dependencies();
    }
    public class Dependencies
    {
        [JsonPropertyName("direct")]
        public Dictionary<string, string> Direct { get; set; } = new Dictionary<string, string>();

        [JsonPropertyName("indirect")]
        public Dictionary<string, string> Indirect { get; set; } = new Dictionary<string, string>();
    }
}