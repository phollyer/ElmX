using ElmX.Core.Console;
using ElmX.Elm.App;
using ElmX.Elm.Code;

namespace ElmX.Elm
{
    public class Application : Elm
    {
        public List<string> SourceDirs { get; private set; } = new();

        public Module EntryModule { get; set; } = new();

        public Core.Json ElmxJson { get; set; } = new();

        public Application(App.Json json, Core.Json elmxJson)
        {
            ElmxJson = elmxJson;

            foreach (string srcDir in json.SourceDirs)
            {
                if (!srcDir.StartsWith(".."))
                {
                    SourceDirs.Add(srcDir);
                }
            }

            ExcludeDirs = ElmxJson.AppJson.ExcludeDirs;
            ExcludeFiles = ElmxJson.AppJson.ExcludeFiles;

            string entryFile = ElmxJson.AppJson.EntryFile;

            Module? entryModule = FindEntryModule(entryFile);

            if (entryModule is not null)
            {
                EntryModule = entryModule;
            }
            else
            {
                Writer.EmptyLine();
                Writer.WriteLine($"I could not find the entry file '{entryFile}' in the following source directories '{string.Join(", ", SourceDirs)}'.");
                Writer.EmptyLine();
                Writer.WriteLine("Exiting...");
                Writer.EmptyLine();
                Environment.Exit(0);
            }
            EntryModule.ParseImports();

            ModulePaths.Add(EntryModule.FilePath);

            foreach (string srcDir in SourceDirs)
            {
                FileList.AddRange(FindAllFiles(srcDir, EntryModule.FilePath, ExcludeDirs));
            }
        }

        private Module? FindEntryModule(string entryFile)
        {
            Module? entryModule = null;

            if (File.Exists(entryFile))
            {
                entryModule = new();
                entryModule.FilePath = entryFile;
            }

            return entryModule;
        }
    }
}