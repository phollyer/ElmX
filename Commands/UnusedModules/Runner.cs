using ElmX.Commands.Options;
using ElmX.Console;
using ElmX.Json;

namespace ElmX.Commands.UnusedModules
{
    static class Runner
    {
        /// <summary>
        /// Run the unused-modules command in the current directory.
        /// </summary>
        /// <param name="options">
        /// The command line options.
        /// </param>
        public static void Run(UnusedModulesOptions options)
        {
            Writer.Clear();

            Elm_Json elmJson = new();
            elmJson.Read();

            if (elmJson.Json == null)
            {
                Writer.EmptyLine();
                Writer.WriteLine("I could not find an elm.json file in the current directory.");
                Environment.Exit(0);
            }

            ElmX_Json elmxJson = new();
            elmxJson.Read();

            if (elmxJson.Json == null)
            {
                Writer.EmptyLine();
                Writer.WriteLine("I could not find an elmx.json file in the current directory. Please run the init command first.");
                Environment.Exit(0);
            }

            if (!options.Show && !options.Delete && !options.Pause && !options.Rename)
            {
                Writer.EmptyLine();
                Writer.WriteLine("You asked me to find the unused modules, but you didn't tell me what to do with them. Please use the -h or --help option to see what your options are.");
                Environment.Exit(0);
            }

            Writer.WriteLine("I will now search for unused modules.");

            Finder? files = null;

            if (options.Show)
            {
                if (elmJson.Json.projectType == ProjectType.Application && elmJson.Json.Application != null)
                {
                    files = new(elmJson.Json.Application.SourceDirs, elmxJson.Json.EntryFile, elmxJson.Json.ExcludedDirs);
                }
                else if (elmJson.Json.projectType == ProjectType.Package && elmJson.Json.Package != null)
                {
                    files = new(elmJson.Json.Package.Src, elmJson.Json.Package.ExposedModules, elmxJson.Json.ExcludedDirs);
                }
                else
                {
                    Writer.EmptyLine();
                    Writer.WriteLine("I could not find the source directories for this project.");
                    Environment.Exit(0);
                }

                if (files != null && files.Unused.Count == 0)
                {
                    Writer.EmptyLine();
                    Writer.WriteLine("I did not find any unused modules.");
                    Environment.Exit(0);
                }
                else if (files != null)
                {

                    Writer.WriteLine("You asked me to show you the unused modules. I will do that now.");

                    ShowUnusedModules(files.Unused);

                    Environment.Exit(0);
                }
            }

            //
            //Directories.Finder dirs = new(options.Dir, options.ExcludedDirs);
            //
            //Finder files = new(options.Dir, elmxJson.Json.EntryFile, dirs.Excluded);
            //
            //if (options.Show && options.Pause)
            //{
            //if (files.Unused.Count == 0)
            //{
            //Writer.EmptyLine();
            //Writer.WriteLine("I did not find any unused modules.");
            //Environment.Exit(0);
            //}
            //
            //Writer.WriteLine("You asked me to show you the unused modules and then pause before deleting them. I will do that now.");
            //
            //ShowUnusedModules(files.Unused);
            //PauseUnusedModules(files.Unused);
            //
            //Environment.Exit(0);
            //}
            //
            //if (options.Show && options.Delete)
            //{
            //if (files.Unused.Count == 0)
            //{
            //Writer.EmptyLine();
            //Writer.WriteLine("I did not find any unused modules.");
            //Environment.Exit(0);
            //}
            //
            //Writer.WriteLine("You asked me to show you the unused modules and then delete them. I will do that now.");
            //
            //ShowUnusedModules(files.Unused);
            //DeleteUnusedModules(files.Unused);
            //
            //Environment.Exit(0);
            //}
            //
            //if (options.Show && options.Rename)
            //{
            //if (files.Unused.Count == 0)
            //{
            //Writer.EmptyLine();
            //Writer.WriteLine("I did not find any unused modules.");
            //Environment.Exit(0);
            //}
            //
            //Writer.WriteLine("You asked me to show you the unused modules and then rename them. I will do that now.");
            //
            //ShowUnusedModules(files.Unused);
            //Rename(files.Unused);
            //
            //Environment.Exit(0);
            //}
            //
            //if (options.Delete)
            //{
            //if (files.Unused.Count == 0)
            //{
            //Writer.EmptyLine();
            //Writer.WriteLine("I did not find any unused modules.");
            //Environment.Exit(0);
            //}
            //
            //Writer.WriteLine("You asked me to delete the unused modules. I will do that now.");
            //
            //DeleteUnusedModules(files.Unused);
            //
            //Environment.Exit(0);
            //}
            //
            //if (options.Pause)
            //{
            //if (files.Unused.Count == 0)
            //{
            //Writer.EmptyLine();
            //Writer.WriteLine("I did not find any unused modules.");
            //Environment.Exit(0);
            //}
            //
            //Writer.WriteLine("You asked me to pause before deleting the unused modules. I will do that now.");
            //
            //PauseUnusedModules(files.Unused);
            //
            //Environment.Exit(0);
            //}
            //
            //if (options.Rename)
            //{
            //if (files.Unused.Count == 0)
            //{
            //Writer.EmptyLine();
            //Writer.WriteLine("I did not find any unused modules.");
            //Environment.Exit(0);
            //}
            //
            //Writer.WriteLine("You asked me to rename the unused modules. I will do that now.");
            //
            //Rename(files.Unused);
            //
            //Environment.Exit(0);
            //}
            //
        }

        /// <summary>
        /// Delete the unused modules.  
        /// </summary>
        /// <param name="files">
        /// The list of unused modules.
        /// </param>
        private static void DeleteUnusedModules(List<string> files)
        {
            foreach (string file in files)
            {
                File.Delete(file);
            }

        }

        /// <summary>
        /// Pause before deleting the unused modules. Request permission before deleting each module.
        /// </summary>
        /// <param name="files">
        /// The list of unused modules.
        /// </param>
        private static void PauseUnusedModules(List<string> files)
        {
            foreach (string file in files)
            {
                MaybeDeleteFile(file);
            }
        }

        /// <summary>
        /// Request permission before deleting a module.
        /// </summary>
        /// <param name="file">
        /// The module to delete.
        /// </param>
        private static void MaybeDeleteFile(string file)
        {
            Writer.EmptyLine();
            Writer.WriteLine($"Should I delete the following file? (y/n/(q)uit)");
            Writer.WriteLine(file);

            string key = Reader.ReadKey();

            if (key == "y")
            {
                File.Delete(file);
                Writer.WriteLine("Deleted.");
            }
            else if (key == "q")
            {
                Writer.EmptyLine();
                Writer.WriteLine("Exiting...");
                Environment.Exit(0);
            }
            else if (key == "n")
            {
                Writer.EmptyLine();
                Writer.WriteLine("Skipping...");
            }
        }

        /// <summary>
        /// Rename the unused modules by prepending a tilde (~) to the filename.
        /// </summary>
        /// <param name="files">
        /// The list of unused modules.
        /// </param>
        private static void Rename(List<string> files)
        {
            foreach (string file in files)
            {
                string? path = Path.GetDirectoryName(file);
                string? filename = Path.GetFileName(file);

                if (filename != null && path != null)
                {
                    string newFile = Path.Combine(path, $"~{filename}");

                    File.Move(file, newFile);

                    Writer.WriteLine($"Renamed: {file} -> ~{filename}");
                }
                else if (filename != null)
                {
                    File.Move(file, "~" + filename);

                    Writer.WriteLine($"Renamed: {file} -> ~{filename}");
                }
                else
                {
                    Writer.WriteLine($"Unable to rename {file}");
                }
            }
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

    public class Finder
    {
        /// <summary>
        /// The list of unused files.
        /// </summary>
        public List<string> Unused { get; set; } = new();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="src">
        /// The directory to search.
        /// </param>
        /// <param name="entryFile">
        /// The entry file of the Elm project.
        /// </param>
        /// <param name="excludedDirs">
        /// The list of directories to exclude.
        /// </param>
        public Finder(string src, List<string> exposedModules, List<string> excludedDirs)
        {
            SearchPackage(src, exposedModules, excludedDirs);
        }

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
        public Finder(List<string> srcDirectories, string entryFile, List<string> excludedDirs)
        {
            SearchApplication searchApplication = new();

            foreach (string dir in srcDirectories)
            {
                searchApplication.Run(dir, entryFile, excludedDirs);
            }
        }

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
        private void SearchPackage(string srcDir, List<string> exposedModules, List<string> excludedDirs)
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