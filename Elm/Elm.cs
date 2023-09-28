using ElmX.Core.Console;
using ElmX.Elm.Code;

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

        protected List<string> FindAllFiles(List<string> sourceDirectories, Module entryModule, List<string> excludedDirs)
        {
            List<string> files = new();

            foreach (string srcDir in sourceDirectories)
            {
                files.AddRange(FindAllFiles(srcDir, entryModule, ExcludeDirs));
            }

            return files;
        }

        protected List<string> FindAllFiles(string srcDir, List<Module> exposedModules, List<string> excludedDirs)
        {
            List<string> files = new();

            foreach (Module module in exposedModules)
            {
                files.AddRange(FindAllFiles(srcDir, module, ExcludeDirs));
            }

            return files;
        }

        private List<string> FindAllFiles(string srcDir, Module topLevelModule, List<string> excludedDirs)
        {
            List<string> files = new();
            try
            {
                IEnumerable<string> _files = from file in Directory.EnumerateFiles(srcDir, "*.elm", SearchOption.AllDirectories)
                                             where IsNotExcluded(file, excludedDirs)
                                             where file != topLevelModule.FilePath
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