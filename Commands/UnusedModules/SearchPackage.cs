using ElmX.Core.Console;

namespace ElmX.Commands.UnusedModules
{
    public class SearchPackage
    {
        public List<string> Unused { get; set; } = new List<string>();

        /// <summary>
        /// Search for all the Elm files in a Package.
        /// </summary>
        /// <param name="srcDir">
        /// The source-directory containing the Elm files for the Package.
        /// </param>
        /// <param name="exposedModules">
        /// The Elm files that are exposed by the Package. These are top level files that are not necessarily imported by any other files in the Package
        /// As a result they should not be considered unused.
        /// </param>
        /// <param name="excludedDirs">
        /// The list of directories to exclude.
        /// </param>
        public void Run(string srcDir, List<string> exposedModules, List<string> excludedDirs)
        {
            try
            {
                var files = from file in Directory.EnumerateFiles(srcDir, "*.elm", SearchOption.AllDirectories)
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
                                 Line = Path.Join(srcDir, line[7..].Split(" ")[0].Replace(".", Path.DirectorySeparatorChar.ToString()) + ".elm"),
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

                Writer.WriteLine($"Searching: {srcDir}");
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
                        if (file == line || exposedModules.Contains(file))
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

                if (Unused.Count == 0)
                {
                    Writer.EmptyLine();
                    Writer.WriteLine("I did not find any unused modules.");
                    Environment.Exit(0);
                }
                else
                {

                    Writer.WriteLine("You asked me to show you the unused modules. I will do that now.");

                    ShowUnusedModules(Unused);

                    Environment.Exit(0);
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

        /// <summary>
        /// Show the unused modules.
        /// </summary>
        /// <param name="files">
        /// The list of unused modules.
        /// </param>
        private static void ShowUnusedModules(List<string> files)
        {
            Writer.WriteLines(files);
        }
    }
}