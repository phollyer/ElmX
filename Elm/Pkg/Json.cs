using System.Text.Json.Serialization;

namespace ElmX.Elm.Pkg
{
    public class Json
    {
        public readonly string Src = "src";

        [JsonPropertyName("type")]
        public string Type { get; set; } = "";

        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("summary")]
        public string Summary { get; set; } = "";

        [JsonPropertyName("license")]
        public string License { get; set; } = "";

        [JsonPropertyName("version")]
        public string Version { get; set; } = "";

        [JsonPropertyName("exposed-modules")]
        public List<string> ExposedModules { get; set; } = new List<string>();

        [JsonPropertyName("elm-version")]
        public string ElmVersion { get; set; } = "";

        [JsonPropertyName("dependencies")]
        public Dictionary<string, string> Dependencies { get; set; } = new();

        [JsonPropertyName("test-dependencies")]
        public Dictionary<string, string> TestDependencies { get; set; } = new();
    }
}