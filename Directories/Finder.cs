// A class that finds all the directory paths for directories that contain an elm.json file.
// It must exclude the elm-stuff, node_modules, review, and tests directories,
// along with any other directories that are specified in the constructor.

namespace ElmX.Directories
{
    public class Finder
    {
        /// <summary>
        /// The list of directories that contain an Elm project.
        /// </summary>
        public List<string> List { get; set; } = new();

        /// <summary>
        /// The list of directories to exclude.
        /// </summary>
        /// <value>
        /// The default list of directories to exclude plus any additional directories that are specified in the constructor.
        /// </value>
        public List<string> Excluded
        {
            get
            {
                foreach (string excludedDir in _defaultExcludes)
                {
                    if (!_excludedDirs.Contains(excludedDir))
                        _excludedDirs.Add(excludedDir);
                }
                return _excludedDirs;
            }
        }

        /// <summary>
        /// The list of directories to exclude by default.
        /// </summary>
        /// <value>
        /// "elm-stuff" and "node_modules"
        /// </value>
        private readonly string[] _defaultExcludes = new string[]
            { "elm-stuff"
            , "node_modules"
            , "review"
            , "tests"
            };

        /// <summary>
        /// The list of directories to exclude.
        /// </summary>
        private readonly List<string> _excludedDirs = new();

        /// <summary>
        /// The directory to search.
        /// </summary>
        private string SearchDir { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="searchDir"></param>
        /// <param name="excludedDirs"></param>
        public Finder(string searchDir, List<string> excludedDirs)
        {
            SearchDir = searchDir;

            foreach (string excludedDir in excludedDirs)
            {
                if (!Excluded.Contains(excludedDir))
                    Excluded.Add(excludedDir);
            }

            List = Search();
        }

        /// <summary>
        /// Search for all the directories that contain an elm.json file.
        /// </summary>
        /// <returns>
        /// A list of directory paths where each one contains an Elm project.
        /// </returns>
        private List<string> Search()
        {
            string[] _files = Directory.GetFiles(SearchDir, "elm.json", SearchOption.AllDirectories);

            List<string> files = RemoveExcludedDirs(_files);

            return ToDirectoryPaths(files);
        }

        /// <summary>
        /// Remove any directories that are excluded.
        /// </summary>
        /// <param name="_files"></param>
        /// <returns>
        /// The list of files with the excluded directories removed.
        /// </returns>
        private List<string> RemoveExcludedDirs(string[] _files)
        {
            List<string> files = new(_files);

            foreach (string file in _files)
            {
                foreach (string excludedDir in Excluded)
                {
                    if (file.Contains(excludedDir))
                    {
                        files.Remove(file);
                    }
                }
            }

            return files;
        }

        /// <summary>
        /// Convert a list of elm.json file paths to their containing directory paths.
        /// </summary>
        /// <param name="files"></param>
        /// <returns>
        /// A list of directory paths where each one contains an Elm project.
        /// </returns>
        private List<string> ToDirectoryPaths(List<string> files)
        {
            List<string> dirs = new();

            foreach (string file in files)
            {
                string? dir = Path.GetDirectoryName(file);

                if (dir != null)
                {
                    dirs.Add(dir);
                }
            }

            return dirs;
        }
    }
}