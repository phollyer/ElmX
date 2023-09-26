using ElmX.Core.Console;
using ElmX.Elm.App;
using ElmX.Elm.Code;

namespace ElmX.Elm
{
    public class Application
    {

        // Metadata
        public Metadata Metadata { get; private set; }

        // Source Directories
        public List<string> SourceDirs { get; private set; } = new();

        // Excluded Directories
        public List<string> ExcludeDirs { get; private set; } = new();

        // Excluded Files
        public List<string> ExcludeFiles { get; private set; } = new();

        // Modules

        public Module EntryModule { get; private set; } = new();
        public List<Module> Modules { get; private set; } = new();

        public List<string> ModulePaths { get; private set; } = new();

        // Imports

        public List<Import> Imports { get; private set; } = new();

        public Application(App.Json json, Core.Json elmxJson)
        {
            Metadata = new Metadata(json);

            foreach (string srcDir in json.SourceDirs)
            {
                if (!srcDir.StartsWith(".."))
                {
                    SourceDirs.Add(srcDir);
                }
            }

            ExcludeDirs = elmxJson.AppJson.ExcludeDirs;
            ExcludeFiles = elmxJson.AppJson.ExcludeFiles;

            string entryFile = elmxJson.AppJson.EntryFile;

            Module? entryModule = FindEntryModule(entryFile);

            if (entryModule is not null)
            {
                EntryModule = entryModule;
                EntryModule.ParseImports();

                ModulePaths.Add(EntryModule.Path);

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

            foreach (Import import in EntryModule.Imports)
            {
                Imports.Add(import);
            }

            foreach (string srcDir in SourceDirs)
            {
                foreach (Import import in Imports)
                {
                    string modulePath = Path.Join(srcDir, import.Name.Replace(".", Path.DirectorySeparatorChar.ToString()) + ".elm");
                    if (File.Exists(modulePath))
                    {
                        Module module = new(modulePath);
                        module.ParseImports();
                        Modules.Add(module);

                        ModulePaths.Add(module.Path);

                        ExtractImports(srcDir, module);
                    }
                }
            }

            ModulePaths.Sort();
        }
        public List<string> FindUnusedModules()
        {
            List<string> allFiles = new();

            foreach (string srcDir in SourceDirs)
            {
                allFiles.AddRange(FindAllFiles(srcDir, EntryModule.Path, ExcludeDirs));
            }

            List<string> Unused = new();

            int fileCount = 0;

            Writer.EmptyLine();
            Writer.WriteLine("Searching Dirs:");
            Writer.WriteLines("\t", SourceDirs);
            Writer.EmptyLine();

            Writer.WriteLine("Exclude Dirs:");
            Writer.WriteLines("\t", ExcludeDirs);
            Writer.EmptyLine();

            Writer.WriteLine("Exclude Files:");
            Writer.WriteLines("\t", ExcludeFiles);
            Writer.EmptyLine();

            Writer.WriteLine($"Found: {ModulePaths.Count()} unique imports");
            Writer.WriteLine($"Found: {allFiles.Count()} files");

            short lineNumberToWriteAt = (short)(10 + (short)SourceDirs.Count() + (short)ExcludeDirs.Count() + (short)ExcludeFiles.Count());

            foreach (var filePath in allFiles)
            {
                fileCount++;

                Writer.WriteAt($"Checking file {fileCount}", 0, lineNumberToWriteAt);
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

                if (!found && !ExcludeFiles.Contains(filePath))
                {
                    Unused.Add(filePath);
                }
            }

            Writer.WriteAt($"Found: {Unused.Count().ToString()} unused files", 0, lineNumberToWriteAt);
            Writer.EmptyLine();

            return Unused;
        }

        private Module? FindEntryModule(string entryFile)
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