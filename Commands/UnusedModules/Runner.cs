using ElmX.Core;
using ElmX.Core.Console;
using ElmX.Elm;

namespace ElmX.Commands.UnusedModules
{
    static class Runner
    {
        static Elm.Json ElmJson = new();

        static Core.Json ElmX_Json = new();

        static Runner()
        {

            ElmJson.Read();

            if (ElmJson.json == null)
            {
                Writer.EmptyLine();
                Writer.WriteLine("I could not find an elm.json file in the current directory.");
                Environment.Exit(0);
            }

            ElmX_Json.Read();

            if (ElmX_Json.AppJson == null && ElmX_Json.PkgJson == null)
            {
                Writer.EmptyLine();
                Writer.WriteLine("I could not find an elmx.json file in the current directory. Please run the init command first.");
                Environment.Exit(0);
            }
        }
        static public void Run(Options options)
        {
            if (options.ShowHelp)
            {
                Help.ShowUnusedModulesOptions();
                Environment.Exit(0);
            }

            if (!options.Show && !options.Delete && !options.Pause && !options.Rename)
            {
                Writer.EmptyLine();
                Writer.WriteLine("You asked me to find the unused modules, but you didn't tell me what to do with them. Please use the -h or --help option to see what your options are.");
                Environment.Exit(0);
            }

            Writer.Clear();
            Writer.WriteLine("I will now search for unused modules.");

            List<string> unusedModules = Run();

            if (unusedModules.Count == 0)
            {
                Writer.EmptyLine();
                Writer.WriteLine("I did not find any unused modules.");
                Environment.Exit(0);
            }

            if (options.Show && options.Rename)
            {
                Writer.EmptyLine();
                Writer.WriteLine("You asked me to show you the unused modules and then rename them. I will do that now.");
                Writer.EmptyLine();
                Writer.WriteLines(unusedModules);

                Rename(unusedModules);

                Environment.Exit(0);
            }

            if (options.Show && options.Pause)
            {
                Writer.EmptyLine();
                Writer.WriteLine("You asked me to show you the unused modules and then pause before deleting them. I will do that now.");
                Writer.EmptyLine();
                Writer.WriteLines(unusedModules);

                PauseUnusedModules(unusedModules);

                Environment.Exit(0);
            }

            if (options.Show && options.Delete)
            {
                Writer.EmptyLine();
                Writer.WriteLine("You asked me to show you the unused modules and then delete them. I will do that now.");
                Writer.EmptyLine();
                Writer.WriteLines(unusedModules);

                DeleteUnusedModules(unusedModules);

                Environment.Exit(0);
            }

            if (options.Pause)
            {
                Writer.EmptyLine();
                Writer.WriteLine("You asked me to pause before deleting the unused modules. I will do that now.");

                PauseUnusedModules(unusedModules);

                Environment.Exit(0);
            }

            if (options.Rename)
            {
                Writer.EmptyLine();
                Writer.WriteLine("You asked me to rename the unused modules. I will do that now.");

                Rename(unusedModules);

                Environment.Exit(0);
            }

            if (options.Show)
            {
                Writer.EmptyLine();
                Writer.WriteLine("You asked me to show you the unused modules. I will do that now.");
                Writer.EmptyLine();
                Writer.WriteLines(unusedModules);

                Environment.Exit(0);
            }

            if (options.Delete)
            {
                Writer.EmptyLine();
                Writer.WriteLine("You asked me to delete the unused modules. I will do that now.");

                DeleteUnusedModules(unusedModules);

                Environment.Exit(0);
            }
        }

        static private List<string> Run()
        {
            if (ElmJson.json.projectType == ProjectType.Application && ElmJson.json.Application != null)
            {
                Elm.Application app = new(ElmJson.json.Application, ElmX_Json);

                List<string> unused =
                    app
                    .FindAllFiles()
                    .FindUnusedModules()
                    ;

                WriteSummary(unused, app.SourceDirs, app.ExcludeDirs, app.ExcludeFiles, app.ModulePaths, app.FileList);

                return unused;
            }
            else if (ElmJson.json.projectType == ProjectType.Package && ElmJson.json.Package != null)
            {
                Elm.Package pkg = new(ElmJson.json.Package, ElmX_Json);

                List<string> unused =
                    pkg
                    .FindAllFiles()
                    .FindUnusedModules()
                    ;

                WriteSummary(unused, new List<string>() { "src" }, pkg.ExcludeDirs, pkg.ExcludeFiles, pkg.ModulePaths, pkg.FileList);

                return unused;
            }
            else
            {
                Writer.EmptyLine();
                Writer.WriteLine("I was unable to find the project type. Please make sure you are in the root of your project and that you have an elm.json file.");
                Writer.WriteLine("If your elm.json file does exist, please open an issue at https://github.com/phollyer/ElmX/issues/new and include your elm.json file.");
                Writer.EmptyLine();
                Writer.WriteLine("Exiting...");
                Environment.Exit(0);

                return new();
            }
        }

        static private void WriteSummary(List<string> unused, List<string> sourceDirs, List<string> excludeDirs, List<string> excludeFiles, List<string> modulePaths, List<string> allFiles)
        {
            Writer.EmptyLine();
            Writer.WriteLine("Searching Dirs:");
            Writer.WriteLines("\t", sourceDirs);
            Writer.EmptyLine();

            Writer.WriteLine("Exclude Dirs:");
            Writer.WriteLines("\t", excludeDirs);
            Writer.EmptyLine();

            Writer.WriteLine("Exclude Files:");
            Writer.WriteLines("\t", excludeFiles);
            Writer.EmptyLine();

            Writer.WriteLine($"Found: {modulePaths.Count()} unique imports");
            Writer.WriteLine($"Found: {allFiles.Count()} files");

            int lineNumberToWriteAt = 10 + sourceDirs.Count() + excludeDirs.Count() + excludeFiles.Count();

            Writer.WriteAt($"Found: {unused.Count} unused files", 0, lineNumberToWriteAt);
            Writer.EmptyLine();
        }

        static private void DeleteUnusedModules(List<string> files)
        {
            foreach (string file in files)
            {
                File.Delete(file);
            }

        }

        static private void PauseUnusedModules(List<string> files)
        {
            foreach (string file in files)
            {
                MaybeDeleteFile(file);
            }
        }

        static private void MaybeDeleteFile(string file)
        {
            Writer.EmptyLine();
            Writer.WriteLine($"Should I delete: {file}? (y/n/(q)uit)");

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
            else
            {
                Writer.WriteLine($"I did not understand your input. Please try again.");
                MaybeDeleteFile(file);
            }
        }

        static private void Rename(List<string> files)
        {
            foreach (string file in files)
            {
                string? path = System.IO.Path.GetDirectoryName(file);
                string? filename = System.IO.Path.GetFileName(file);

                if (filename != null && path != null)
                {
                    string newFile = System.IO.Path.Combine(path, $"~{filename}");

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
    }
}