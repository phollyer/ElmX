using ElmX.Core.Console;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ElmX.Core
{
    public class Json
    {
        public ElmXJson_Application AppJson = new();

        public ElmXJson_Package PkgJson = new();

        public readonly bool Exists = false;

        public Json()
        {
            Exists = File.Exists("elmx.json");
        }

        public void Create(Commands.Init.Options options)
        {
            ElmX.Elm.Json elmJson = new();
            elmJson.Read();

            if (elmJson.json.projectType == Elm.ProjectType.Application)
            {
                AppJson.EntryFile = options.EntryFile;
                AppJson.ExcludeDirs = options.ExcludeDirs;
                AppJson.ExcludeFiles = options.ExcludeFiles;

                string jsonStr = JsonSerializer.Serialize(AppJson, new JsonSerializerOptions { WriteIndented = true });

                File.WriteAllText("elmx.json", jsonStr);
            }
            else if (elmJson.json.projectType == Elm.ProjectType.Package)
            {
                PkgJson.ExcludeDirs = options.ExcludeDirs;
                PkgJson.ExcludeFiles = options.ExcludeFiles;

                string jsonStr = JsonSerializer.Serialize(PkgJson, new JsonSerializerOptions { WriteIndented = true });

                File.WriteAllText("elmx.json", jsonStr);
            }
        }

        public void Read()
        {
            try
            {
                string jsonStr = File.ReadAllText("elmx.json");

                ElmXJson_Application? _json = JsonSerializer.Deserialize<ElmXJson_Application>(jsonStr);

                if (_json != null)
                {
                    AppJson = _json;
                }
                else
                {
                    Writer.WriteLine("There was an error reading the elmx.json file.");
                    Environment.Exit(1);
                }
            }
            catch (FileNotFoundException)
            {
                Writer.WriteLine("The elmx.json file does not exist.");
                Writer.EmptyLine();
                Writer.WriteLine("Run 'elmx init' to create a new elmx.json file with default values. (You can edit the default values manually yourself, later).");
                Writer.EmptyLine();
                Writer.WriteLine("Alternatively,");
                Writer.EmptyLine();
                Writer.WriteLine("Run 'elmx init -h' for more information.");
                Writer.EmptyLine();
                Writer.WriteLine("Exiting...");
                Environment.Exit(1);
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
    public class ElmXJson_Application
    {
        [JsonPropertyName("entry-file")]
        public string EntryFile { get; set; } = "";

        [JsonPropertyName("exclude-dirs")]
        public List<string> ExcludeDirs { get; set; } = new List<string>();

        [JsonPropertyName("exclude-files")]
        public List<string> ExcludeFiles { get; set; } = new List<string>();
    }
    public class ElmXJson_Package
    {
        [JsonPropertyName("exclude-dirs")]
        public List<string> ExcludeDirs { get; set; } = new List<string>();

        [JsonPropertyName("exclude-files")]
        public List<string> ExcludeFiles { get; set; } = new List<string>();
    }
}