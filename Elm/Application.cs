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

        // External Packages
        public List<Package> Packages { get; private set; } = new();

        public Application(App.Json json, Core.Json elmxJson)
        {
            Metadata = new Metadata(json);

            SourceDir = json.SourceDirs[0];

            ExcludedDirs = elmxJson.json.ExcludedDirs;

            FindModules(SourceDir, elmxJson.json.EntryFile, ExcludedDirs);
        }

        public List<string> FindUnusedModules()
        {
            List<string> importPaths = new();

            foreach (Import import in EntryModule.Imports)
            {
                string importPath = Path.Join(SourceDir, import.Name.Replace(".", Path.DirectorySeparatorChar.ToString()) + ".elm");
                importPaths.Add(importPath);
            }

            foreach (Module module in Modules)
            {
                foreach (Import import in module.Imports)
                {
                    string importPath = Path.Join(SourceDir, import.Name.Replace(".", Path.DirectorySeparatorChar.ToString()) + ".elm");
                    importPaths.Add(importPath);
                }
            }

            importPaths = importPaths.Distinct().ToList();
            importPaths.Sort();

            List<string> modulePaths = new();

            foreach (Module module in Modules)
            {
                modulePaths.Add(module.Path);
            }

            modulePaths.Sort();

            List<string> Unused = new();

            int fileCount = 0;

            Writer.EmptyLine();
            Writer.WriteLine($"Searching: {SourceDir}");
            Writer.WriteLine($"Excluding: {string.Join(", ", ExcludedDirs)}");
            Writer.WriteLine($"Found: {importPaths.Count()} unique imports");
            Writer.WriteLine($"Found: {modulePaths.Count()} files");

            foreach (var filePath in modulePaths)
            {
                fileCount++;

                Writer.WriteAt($"Checking file {fileCount}", 0, 6);
                Writer.EmptyLine();

                bool found = false;
                foreach (var importPath in importPaths)
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

        private void FindModules(string srcDir, string entryFile, List<string> excludedDirs)
        {
            try
            {
                var files = from file in Directory.EnumerateFiles(srcDir, "*.elm", SearchOption.AllDirectories)
                            where IsNotExcluded(file, excludedDirs)
                            select new
                            {
                                File = file,
                            };

                string entryFilePath = Path.Join(srcDir, entryFile);

                foreach (var file in files)
                {
                    if (entryFilePath == file.File)
                    {
                        EntryModule = new Module(entryFilePath);
                        EntryModule.ParseImports();
                    }
                    else
                    {
                        Module module = new(file.File);
                        module.ParseImports();
                        Modules.Add(module);
                    }
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