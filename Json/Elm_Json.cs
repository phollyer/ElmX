// A class that  reads an elm.json file.

using ElmX.Console;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ElmX.Json
{
    enum ProjectType
    {
        Application,
        Package
    }

    class Type
    {
        [JsonPropertyName("type")]
        public string type { get; set; } = "";
    }

    class Application
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
    class Dependencies
    {
        [JsonPropertyName("direct")]
        public Dictionary<string, string> Direct { get; set; } = new Dictionary<string, string>();

        [JsonPropertyName("indirect")]
        public Dictionary<string, string> Indirect { get; set; } = new Dictionary<string, string>();
    }

    class Package
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

    class ElmJson
    {
        public ProjectType projectType;

        public Application? Application;

        public Package? Package;
    }

    class Elm_Json
    {
        public ElmJson Json = new();

        public Elm_Json()
        {
            if (!File.Exists("elm.json"))
            {
                Writer.EmptyLine();
                Writer.WriteLine("I could not find an elm.json file in the current directory.");
                Writer.EmptyLine();
                Writer.WriteLine("Exiting...");
                Writer.EmptyLine();
                Environment.Exit(0);
            }
        }

        public void Read()
        {
            string jsonStr = File.ReadAllText("elm.json");

            try
            {
                Type? Type = JsonSerializer.Deserialize<Type>(jsonStr);

                if (Type != null)
                {
                    if (Type.type == "application")
                    {
                        Json.projectType = ProjectType.Application;

                        Application? applicaton = JsonSerializer.Deserialize<Application>(jsonStr);

                        if (applicaton != null)
                        {
                            Json.Application = applicaton;
                        }
                    }
                    else if (Type.type == "package")
                    {
                        Json.projectType = ProjectType.Package;

                        Package? package = JsonSerializer.Deserialize<Package>(jsonStr);

                        if (package != null)
                        {
                            Json.Package = package;
                        }
                    }
                    else
                    {
                        Writer.EmptyLine();
                        Writer.WriteLine($"This project is of type '{Type.type}'.");
                        Writer.WriteLine("But I only know about Application and Package projects.");
                        Writer.EmptyLine();
                        Writer.WriteLine($"If {Type.type} as a valid Elm project, please raise an issue and I'll add support for it.");
                        Writer.WriteLine("Exiting...");
                        Writer.EmptyLine();
                        Environment.Exit(0);
                    }
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