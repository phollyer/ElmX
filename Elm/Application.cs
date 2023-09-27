using ElmX.Core.Console;
using ElmX.Elm.App;
using ElmX.Elm.Code;

namespace ElmX.Elm
{
    public class Application : Elm
    {
        public Metadata Metadata { get; private set; }

        public List<string> SourceDirs { get; private set; } = new();

        public Module EntryModule { get; set; } = new();

        public Core.Json ElmxJson { get; set; } = new();

        public Application(App.Json json, Core.Json elmxJson)
        {
            Metadata = new Metadata(json);

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

            foreach (string srcDir in SourceDirs)
            {
                FileList.AddRange(FindAllFiles(srcDir, EntryModule.Path, ExcludeDirs));
            }
        }

        public Dictionary<string, List<string>> FindUnusedImports()
        {
            Dictionary<string, List<string>> unusedImports = new();

            List<string> allFiles = new();

            foreach (string srcDir in SourceDirs)
            {
                allFiles.AddRange(FindAllFiles(srcDir, EntryModule.Path, ExcludeDirs));
            }

            allFiles.Sort();

            foreach (string filePath in allFiles)
            {
                Module module = new(filePath);
                module.ParseImports();

                Modules.Add(module);
            }

            return unusedImports;
        }

        public Module? FindEntryModule(string entryFile)
        {
            Module? entryModule = null;

            if (File.Exists(entryFile))
            {
                entryModule = new(entryFile);

            }

            return entryModule;
        }

        private void ExtractImports(string srcDir, Module module)
        {
            foreach (Import import in module.Imports)
            {
                string modulePath = Path.Join(srcDir, import.Name.Replace(".", Path.DirectorySeparatorChar.ToString()) + ".elm");

                if (ModulePaths.IndexOf(modulePath) == -1 && File.Exists(modulePath))
                {
                    Module _module = new(modulePath);
                    _module.ParseImports();
                    Modules.Add(_module);

                    ModulePaths.Add(_module.Path);

                    ExtractImports(srcDir, _module);
                }
            }
        }

    }
}