using ElmX.Core.Console;
using ElmX.Elm.Code;
using ElmX.Core;

namespace ElmX.Elm
{
    public class Elm
    {
        public List<string> ExcludeDirs { get; protected set; } = new();

        public List<string> ExcludeFiles { get; protected set; } = new();

        public List<Module> Modules { get; protected set; } = new();

        public List<string> ModulePaths { get; protected set; } = new();

        public List<Import> Imports { get; private set; } = new();

        public List<string> FileList { get; protected set; } = new();


        protected List<string> FindUnusedModules()
        {
            List<string> unused = new();

            foreach (var filePath in FileList)
            {
                bool found = false;
                foreach (var importPath in ModulePaths)
                {
                    if (filePath == importPath)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    unused.Add(filePath);
                }
            }

            return unused;
        }

        protected Dictionary<string, List<string>> FindUnusedImports()
        {
            ModulesFromPaths();

            Dictionary<string, List<string>> unused = new();

            foreach (Module module in Modules)
            {
                if (module.Name == "App")
                {
                    Writer.WriteLine($"Parsing: {module.Name}");
                    module.ParseImports();
                    module.ParseTypeAliases();
                }

            }

            return unused;
        }
        protected void ModulesFromImports(string srcDir)
        {
            foreach (Import import in Imports)
            {
                string modulePath = ModulePathFromDotNotation(srcDir, import.Name);

                if (File.Exists(modulePath))
                {
                    Module module = ModuleFromPath(modulePath);
                    module.ParseImports();

                    Modules.Add(module);

                    ModulePaths.Add(module.FilePath);

                    CreateModuleList(srcDir, module);
                }
            }

            ModulePaths.Sort();
        }

        protected void ModulesFromPaths()
        {
            foreach (string filePath in FileList)
            {
                Module module = new(filePath);

                Modules.Add(module);
            }
        }

        private Module ModuleFromPath(string path)
        {
            Module module = new(path);

            return module;
        }

        private void CreateModuleList(string srcDir, Module module)
        {
            foreach (Import import in module.Imports)
            {
                string modulePath = ModulePathFromDotNotation(srcDir, import.Name);

                if (ModulePaths.IndexOf(modulePath) == -1 && File.Exists(modulePath))
                {
                    Module _module = ModuleFromPath(modulePath);
                    _module.ParseImports();

                    Modules.Add(_module);

                    ModulePaths.Add(modulePath);

                    CreateModuleList(srcDir, _module);
                }
            }
        }
        private bool ImportIsUsedByName(string content, string name)
        {
            return
                content.Contains($": {name} ")
                || content.Contains($"-> {name}\n")
                || content.Contains($"\n{name} ")
                || content.Contains($"\n{name}\n")
                ;
        }
        private bool ImportIsUsedAsName(string content, string _as)
        {
            return
            // followed by a space
                content.Contains($": {_as} ")
                || content.Contains($"-> {_as} ")
                || content.Contains($"\t{_as} ")

            // followed by a dot
                || content.Contains($": {_as}.")
                || content.Contains($"-> {_as}.")
                || content.Contains($" {_as}.")
                || content.Contains($"\t{_as}.")
                || content.Contains($"\n{_as}.");
            ;
        }

        private bool ImportIsUsedByExposing(string content, List<string> exposing)
        {
            foreach (string func in exposing)
            {

            }

            return false;
        }


        protected List<string> FindAllFiles(List<string> sourceDirectories)
        {
            List<string> files = new();

            foreach (string srcDir in sourceDirectories)
            {
                files.AddRange(FindAllFiles(srcDir));
            }

            return files;
        }

        protected List<string> FindAllFiles(string srcDir)
        {
            List<string> files = new();
            try
            {
                IEnumerable<string> _files = from file in Directory.EnumerateFiles(srcDir, "*.elm", SearchOption.AllDirectories)
                                             where IsNotExcludedDirectory(file)
                                             where IsNotExcludedFile(file)
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

        private string ModulePathFromDotNotation(string srcDir, string dotNotation)
        {
            return System.IO.Path.Join(srcDir, Core.Path.FromDotNotation(dotNotation));
        }

        private bool IsNotExcludedDirectory(string filePath)
        {
            foreach (string excludedDir in ExcludeDirs)
            {
                if (System.IO.Path.GetDirectoryName(filePath) == excludedDir)
                {
                    return false;
                }
            }

            return true;
        }
        private bool IsNotExcludedFile(string filePath)
        {
            foreach (string excludedFile in ExcludeFiles)
            {
                if (filePath == excludedFile)
                {
                    return false;
                }
            }

            return true;
        }
    }
}