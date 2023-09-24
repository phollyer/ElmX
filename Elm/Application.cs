using ElmX.Core.Console;
using ElmX.Elm.App;
using ElmX.Elm.Code;

namespace ElmX.Elm
{
    public class Application
    {
        // Source Directory
        public string SourceDir { get; private set; } = "";

        // Excluded Directories
        public List<string> ExcludedDirs { get; private set; } = new();

        // Metadata
        public Metadata Metadata { get; private set; }

        // Modules

        public Module EntryModule { get; private set; } = new();
        public List<Module> Modules { get; private set; } = new();

        public List<string> ModulePaths { get; private set; } = new();

        // Imports

        public List<Import> Imports { get; private set; } = new();

        // External Packages
        public List<Package> Packages { get; private set; } = new();

        public Application(App.Json json, Core.Json elmxJson)
        {
            Metadata = new Metadata(json);

            SourceDir = json.SourceDirs[0];

            ExcludedDirs = elmxJson.json.ExcludedDirs;

            string entryFile = elmxJson.json.EntryFile;

            Module? entryModule = FindEntryModule(SourceDir, entryFile);

            if (entryModule is not null)
            {
                EntryModule = entryModule;
                EntryModule.ParseImports();

                ModulePaths.Add(EntryModule.Path);

            }
            else
            {
                Writer.EmptyLine();
                Writer.WriteLine($"I could not find the entry file '{entryFile}' in the source directory '{SourceDir}'.");
                Writer.EmptyLine();
                Writer.WriteLine("Exiting...");
                Writer.EmptyLine();
                Environment.Exit(0);
            }

            foreach (Import import in EntryModule.Imports)
            {
                Imports.Add(import);
            }

            foreach (Import import in Imports)
            {
                string modulePath = Path.Join(SourceDir, import.Name.Replace(".", Path.DirectorySeparatorChar.ToString()) + ".elm");
                if (File.Exists(modulePath))
                {
                    Module module = new(modulePath);
                    module.ParseImports();
                    Modules.Add(module);

                    ModulePaths.Add(module.Path);

                    ExtractImports(module);
                }
            }

            ModulePaths.Sort();
        }

        private Module? FindEntryModule(string srcDir, string entryFile)
        {
            string entryFilePath = Path.Join(srcDir, entryFile);

            Module? entryModule = null;

            if (File.Exists(entryFilePath))
            {
                entryModule = new(entryFilePath);

            }

            return entryModule;
        }

        private void ExtractImports(Module module)
        {
            foreach (Import import in module.Imports)
            {
                string modulePath = Path.Join(SourceDir, import.Name.Replace(".", Path.DirectorySeparatorChar.ToString()) + ".elm");

                if (ModulePaths.IndexOf(modulePath) == -1 && File.Exists(modulePath))
                {
                    Module _module = new(modulePath);
                    _module.ParseImports();
                    Modules.Add(_module);

                    ModulePaths.Add(_module.Path);

                    ExtractImports(_module);

                }
            }
        }

        public List<string> FindUnusedModules()
        {
            List<string> allFiles = FindAllFiles(SourceDir, EntryModule.Path, ExcludedDirs);

            List<string> Unused = new();

            int fileCount = 0;

            Writer.EmptyLine();
            Writer.WriteLine($"Searching: {SourceDir}");
            Writer.WriteLine($"Excluding: {string.Join(", ", ExcludedDirs)}");
            Writer.WriteLine($"Found: {ModulePaths.Count()} unique imports");
            Writer.WriteLine($"Found: {allFiles.Count()} files");

            foreach (var filePath in allFiles)
            {
                fileCount++;

                Writer.WriteAt($"Checking file {fileCount}", 0, 6);
                Writer.EmptyLine();

                bool found = false;
                foreach (var importPath in ModulePaths)
                {
                    if (filePath == importPath || filePath == EntryModule.Path)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    Unused.Add(filePath);
                }
            }

            Writer.WriteAt($"Found: {Unused.Count().ToString()} unused files", 0, 6);
            Writer.EmptyLine();

            return Unused;
        }

        private List<string> FindAllFiles(string srcDir, string entryFile, List<string> excludedDirs)
        {
            List<string> files = new();
            try
            {
                IEnumerable<string> _files = from file in Directory.EnumerateFiles(srcDir, "*.elm", SearchOption.AllDirectories)
                                             where IsNotExcluded(file, excludedDirs)
                                             where file != entryFile
                                             select file;

                foreach (string file in _files)
                {
                    files.Add(file);
                }
            }
            catch (DirectoryNotFoundException dirEx)
            {
                Writer.WriteLine(dirEx.Message);
            }
            catch (UnauthorizedAccessException uAEx)
            {
                Writer.WriteLine(uAEx.Message);
            }
            catch (PathTooLongException pathEx)
            {
                Writer.WriteLine(pathEx.Message);
            }

            files.Sort();

            return files;
        }

        /// <summary>
        /// Check if a file is in an excluded directory.
        /// </summary>
        /// <param name="file">
        /// The file to check.
        /// </param>
        /// <param name="excludedDirs">
        /// The list of excluded directories.
        /// </param>
        /// <returns>
        /// True if the file is not in an excluded directory.
        /// </returns>
        private bool IsNotExcluded(string file, List<string> excludedDirs)
        {
            string seperator = Path.DirectorySeparatorChar.ToString();

            foreach (string excludedDir in excludedDirs)
            {
                string searchStr = $"{seperator}{excludedDir}{seperator}";
                if (file.Contains(searchStr))
                {
                    return false;
                }
            }

            return true;
        }
    }
}