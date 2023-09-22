using ElmX.Core.Console;
using ElmX.Elm.App;
using ElmX.Elm.Code;

namespace ElmX.Elm
{
    public class Application
    {
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

            foreach (string dir in json.SourceDirs)
            {
                SearchFiles(dir, elmxJson.json.EntryFile, elmxJson.json.ExcludedDirs);
            }
        }

        private void SearchFiles(string srcDir, string entryFile, List<string> excludedDirs)
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
                        Writer.WriteLine($"Entry module: {EntryModule.ToString()}");
                    }
                    else
                    {
                        Module module = new(file.File);
                        module.ParseImports();
                        Modules.Add(module);
                        Writer.WriteLine($"Entry module: {module.ToString()}");
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