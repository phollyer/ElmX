using ElmX.Core;
using ElmX.Core.Console;
using ElmX.Elm;

namespace ElmX.Commands.UnusedModules
{
    class Runner : RunnerBase
    {
        public void Run(Options options)
        {
            if (options.ShowHelp)
            {
                Help.ShowUnusedModulesOptions();
                Environment.Exit(0);
            }

            List<string> unusedModules = new();

            if (!options.Show && !options.Delete && !options.Pause && !options.Rename)
            {
                Writer.EmptyLine();
                Writer.WriteLine("You asked me to find the unused modules, but you didn't tell me what to do with them. Please use the -h or --help option to see what your options are.");
                Environment.Exit(0);
            }

            Writer.Clear();
            Writer.WriteLine("I will now search for unused modules.");

            unusedModules = Run();

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

        private List<string> Run()
        {

            if (ElmJson.json.projectType == ProjectType.Application && ElmJson.json.Application != null)
            {
                Elm.Application application = new(ElmJson.json.Application, ElmX_Json);

                return Modules.FindUnused(application);
            }
            else if (ElmJson.json.projectType == ProjectType.Package && ElmJson.json.Package != null)
            {
                Elm.Package package = new(ElmJson.json.Package, ElmX_Json);

                return Modules.FindUnused(package);
            }
            else
            {
                return new();
            }
        }

        private void DeleteUnusedModules(List<string> files)
        {
            foreach (string file in files)
            {
                File.Delete(file);
            }

        }

        private void PauseUnusedModules(List<string> files)
        {
            foreach (string file in files)
            {
                MaybeDeleteFile(file);
            }
        }

        private void MaybeDeleteFile(string file)
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

        private void Rename(List<string> files)
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