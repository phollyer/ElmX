using ElmX.Core.Console;
using ElmX.Elm.Code;
using ElmX.Elm.Pkg;

namespace ElmX.Elm
{
    public class Package
    {
        // Metadata
        public Metadata Metadata { get; private set; }


        // Excluded Directories
        public List<string> ExcludeDirs { get; private set; } = new();

        // Excluded Files
        public List<string> ExcludeFiles { get; private set; } = new();

        // Modules

        public List<Module> ExposedModules { get; private set; } = new();

        public List<Module> Modules { get; private set; } = new();

        public List<string> ModulePaths { get; private set; } = new();

        // Imports

        public List<Import> Imports { get; private set; } = new();

        public Package(Pkg.Json json, Core.Json elmxJson)
        {
            Metadata = new Metadata(json);

            ExcludeDirs = elmxJson.AppJson.ExcludeDirs.ToList();
            ExcludeFiles = elmxJson.AppJson.ExcludeFiles.ToList();

            foreach (string exposedModule in Metadata.ExposedModules)
            {
                string modulePath = Path.Join("src", exposedModule.Replace(".", Path.DirectorySeparatorChar.ToString()) + ".elm");

                Module module = new(modulePath);
                module.ParseImports();

                foreach (Import import in module.Imports)
                {
                    Imports.Add(import);
                }

                ModulePaths.Add(module.Path);
            }

            foreach (Import import in Imports)
            {
                string modulePath = Path.Join("src", import.Name.Replace(".", Path.DirectorySeparatorChar.ToString()) + ".elm");
                if (File.Exists(modulePath))
                {
                    Module module = new(modulePath);
                    module.ParseImports();
                    Modules.Add(module);

                    ModulePaths.Add(module.Path);

                    ExtractImports("src", module);
                }
            }

            ModulePaths.Sort();
        }
        public List<string> FindUnusedModules()
        {
            List<string> allFiles = FindAllFiles(ExposedModules.Select(module => module.Path).ToList(), ExcludeDirs);

            List<string> Unused = new();

            int fileCount = 0;

            Writer.EmptyLine();
            Writer.WriteLine("Searching Dir:");
            Writer.WriteLine("\tsrc");
            Writer.EmptyLine();

            Writer.WriteLine("Exclude Dirs:");
            Writer.WriteLines("\t", ExcludeDirs);
            Writer.EmptyLine();

            Writer.WriteLine("Exclude Files:");
            Writer.WriteLines("\t", ExcludeFiles);
            Writer.EmptyLine();

            Writer.WriteLine($"Found: {ModulePaths.Count()} unique imports");
            Writer.WriteLine($"Found: {allFiles.Count()} files");

            short lineNumberToWriteAt = (short)(10 + (short)ExcludeDirs.Count() + (short)ExcludeFiles.Count());

            foreach (var filePath in allFiles)
            {
                fileCount++;

                Writer.WriteAt($"Checking file {fileCount}", 0, lineNumberToWriteAt);
                Writer.EmptyLine();

                bool found = false;
                foreach (var importPath in ModulePaths)
                {
                    if (filePath == importPath || ExposedModules.Select(module => module.Path).Contains(filePath))
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

        private List<string> FindAllFiles(List<string> exposedModules, List<string> excludedDirs)
        {
            List<string> files = new();
            try
            {
                IEnumerable<string> _files = from file in Directory.EnumerateFiles("src", "*.elm", SearchOption.AllDirectories)
                                             where IsNotExcluded(file, excludedDirs)
                                             where !exposedModules.Contains(file)
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