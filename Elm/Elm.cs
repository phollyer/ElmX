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

        protected void ModulesFromImports(List<Module> modules, List<string> modulePaths, string srcDir, List<Import> imports)
        {
            foreach (Import import in imports)
            {
                string modulePath = ModulePathFromDotNotation(srcDir, import.Name);

                if (File.Exists(modulePath))
                {
                    Module module = ModuleFromPath(modulePath);

                    modules.Add(module);

                    modulePaths.Add(module.FilePath);

                    CreateModuleList(modules, modulePaths, srcDir, module);
                }
            }

            modulePaths.Sort();
        }

        private Module ModuleFromPath(string path)
        {
            Module module = new(path);

            module.ParseImports();

            return module;
        }

        private void CreateModuleList(List<Module> list, List<string> paths, string srcDir, Module module)
        {
            foreach (Import import in module.Imports)
            {
                string modulePath = ModulePathFromDotNotation(srcDir, import.Name);

                if (paths.IndexOf(modulePath) == -1 && File.Exists(modulePath))
                {
                    Module _module = ModuleFromPath(modulePath);

                    list.Add(_module);

                    paths.Add(modulePath);

                    CreateModuleList(list, paths, srcDir, _module);
                }
            }
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