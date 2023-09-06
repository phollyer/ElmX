// A class that finds all Elm files in a given directory and its subdirectories
// and returns a list of all the imported modules in those files.

using ElmX.Console;

namespace ElmX.UnusedModules
{
    public class Finder
    {
        /// <summary>
        /// The list of unused files.
        /// </summary>
        public List<string> Unused { get; set; } = new();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dir">
        /// The directory to search.
        /// </param>
        /// <param name="entryFile">
        /// The entry file of the Elm project.
        /// </param>
        /// <param name="excludedDirs">
        /// The list of directories to exclude.
        /// </param>
        public Finder(string dir, string entryFile, List<string> excludedDirs)
        {
            Search(dir, entryFile, excludedDirs);
        }

        /// <summary>
        /// Search for all the Elm files in a given directory and its subdirectories.
        /// </summary>
        /// <param name="dir">
        /// The directory to search.
        /// </param>
        /// <param name="entryFile">
        /// The entry file of the Elm project.
        /// </param>
        /// <param name="excludedDirs">
        /// The list of directories to exclude.
        /// </param>
        private void Search(string dir, string entryFile, List<string> excludedDirs)
        {
            try
            {
                var files = from file in Directory.EnumerateFiles(dir, "*.elm", SearchOption.AllDirectories)
                            where IsNotExcluded(file, excludedDirs)
                            select new
                            {
                                File = file,
                            };

                var _lines = from file in files
                             from line in File.ReadLines(file.File)
                             where line.Contains("import")
                             select new
                             {
                                 Line = line[7..].Split(" ")[0].Replace(".", Path.DirectorySeparatorChar.ToString()) + ".elm",
                             };

                List<string> sortedLines = new();

                foreach (var line in _lines.Distinct())
                {
                    sortedLines.Add(line.Line);
                }

                sortedLines.Sort();

                List<string> sortedFiles = new();

                foreach (var file in files)
                {
                    sortedFiles.Add(file.File);
                }

                sortedFiles.Sort();

                List<string> unusedFiles = new();

                Writer.WriteLine($"Searching: {dir}");
                Writer.WriteLine($"Excluding: {string.Join(", ", excludedDirs)}");
                Writer.WriteLine($"Found: {sortedLines.Count()} unique imports");
                Writer.WriteLine($"Found: {sortedFiles.Count()} files");

                int fileCount = 0;

                foreach (var file in sortedFiles)
                {
                    fileCount++;

                    Writer.WriteAt($"Checking file {fileCount}", 0, 4);
                    Writer.WriteLine("");

                    bool found = false;
                    foreach (var line in sortedLines)
                    {
                        if (file.Contains(line) || file.Contains(entryFile))
                        {
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        Unused.Add(file);
                    }
                }

                Writer.WriteAt($"Found: {Unused.Count().ToString()} unused files", 0, 4);
                Writer.WriteLine("");
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

