using ElmX.Console;
using ElmX.Json;
using ElmX.Options;

namespace ElmX.UnusedModules
{
    static class Runner
    {
        /// <summary>
        /// Run the program.
        /// </summary>
        /// <param name="options">
        /// The command line options.
        /// </param>
        public static void Run(Parser options)
        {

            if (!options.Show && !options.Delete && !options.Pause && !options.Rename)
            {
                Writer.EmptyLine();
                Writer.WriteLine("You asked me to find the unused modules, but you didn't tell me what to do with them. Please use the -h or --help option to see what your options are.");
                Environment.Exit(0);
            }

            Writer.Clear();

            ElmxJson.Read(options.Dir);

            Directories.Finder dirs = new(options.Dir, options.ExcludedDirs);

            Finder files = new(options.Dir, ElmxJson.Entry, dirs.Excluded);

            if (options.Show && options.Pause)
            {
                if (files.Unused.Count == 0)
                {
                    Writer.EmptyLine();
                    Writer.WriteLine("I did not find any unused modules.");
                    Environment.Exit(0);
                }

                Writer.WriteLine("You asked me to show you the unused modules and then pause before deleting them. I will do that now.");

                ShowUnusedModules(files.Unused);
                PauseUnusedModules(files.Unused);

                Environment.Exit(0);
            }

            if (options.Show && options.Delete)
            {
                if (files.Unused.Count == 0)
                {
                    Writer.EmptyLine();
                    Writer.WriteLine("I did not find any unused modules.");
                    Environment.Exit(0);
                }

                Writer.WriteLine("You asked me to show you the unused modules and then delete them. I will do that now.");

                ShowUnusedModules(files.Unused);
                DeleteUnusedModules(files.Unused);

                Environment.Exit(0);
            }

            if (options.Show && options.Rename)
            {
                if (files.Unused.Count == 0)
                {
                    Writer.EmptyLine();
                    Writer.WriteLine("I did not find any unused modules.");
                    Environment.Exit(0);
                }

                Writer.WriteLine("You asked me to show you the unused modules and then rename them. I will do that now.");

                ShowUnusedModules(files.Unused);
                Rename(files.Unused);

                Environment.Exit(0);
            }

            if (options.Delete)
            {
                if (files.Unused.Count == 0)
                {
                    Writer.EmptyLine();
                    Writer.WriteLine("I did not find any unused modules.");
                    Environment.Exit(0);
                }

                Writer.WriteLine("You asked me to delete the unused modules. I will do that now.");

                DeleteUnusedModules(files.Unused);

                Environment.Exit(0);
            }

            if (options.Pause)
            {
                if (files.Unused.Count == 0)
                {
                    Writer.EmptyLine();
                    Writer.WriteLine("I did not find any unused modules.");
                    Environment.Exit(0);
                }

                Writer.WriteLine("You asked me to pause before deleting the unused modules. I will do that now.");

                PauseUnusedModules(files.Unused);

                Environment.Exit(0);
            }

            if (options.Rename)
            {
                if (files.Unused.Count == 0)
                {
                    Writer.EmptyLine();
                    Writer.WriteLine("I did not find any unused modules.");
                    Environment.Exit(0);
                }

                Writer.WriteLine("You asked me to rename the unused modules. I will do that now.");

                Rename(files.Unused);

                Environment.Exit(0);
            }

            if (options.Show)
            {
                if (files.Unused.Count == 0)
                {
                    Writer.EmptyLine();
                    Writer.WriteLine("I did not find any unused modules.");
                    Environment.Exit(0);
                }

                Writer.WriteLine("You asked me to show you the unused modules. I will do that now.");

                ShowUnusedModules(files.Unused);

                Environment.Exit(0);
            }
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
}