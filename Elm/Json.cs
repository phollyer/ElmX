using ElmX.Core.Console;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ElmX.Elm
{
    public enum ProjectType
    {
        Application,
        Package
    }

    public class Json
    {
        public ElmJson json = new();

        public Json()
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
                        json.projectType = ProjectType.Application;

                        App.Json? applicaton = JsonSerializer.Deserialize<App.Json>(jsonStr);

                        if (applicaton != null)
                        {
                            json.Application = applicaton;
                        }
                    }
                    else if (Type.type == "package")
                    {
                        json.projectType = ProjectType.Package;

                        Pkg.Json? package = JsonSerializer.Deserialize<Pkg.Json>(jsonStr);

                        if (package != null)
                        {
                            json.Package = package;
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
    public class Type
    {
        [JsonPropertyName("type")]
        public string type { get; set; } = "";
    }

    public class ElmJson
    {
        public ProjectType projectType;

        public App.Json? Application;

        public Pkg.Json? Package;
    }

}