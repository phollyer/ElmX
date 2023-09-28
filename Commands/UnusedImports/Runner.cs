using ElmX.Core;
using ElmX.Core.Console;
using ElmX.Elm;

namespace ElmX.Commands.UnusedImports
{
    class Runner : RunnerBase
    {
        public void Run(Options options)
        {
            if (options.ShowHelp)
            {
                Help.ShowUnusedImportsOptions();
                Environment.Exit(0);
            }

            Dictionary<string, List<string>> unusedImports = new();

            if (!options.Show && !options.Delete && !options.Pause && !options.Rename)
            {
                Writer.EmptyLine();
                Writer.WriteLine("You asked me to find the unused imports, but you didn't tell me what to do with them. Please use the -h or --help option to see what your options are.");
                Environment.Exit(0);
            }

            Writer.Clear();
            Writer.WriteLine("I will now search for unused imports.");

            if (ElmJson.json.projectType == ProjectType.Application && ElmJson.json.Application != null)
            {
                Elm.Application application = new(ElmJson.json.Application, ElmX_Json);
                unusedImports = Imports.FindUnused(application);
            }
            else if (ElmJson.json.projectType == ProjectType.Package && ElmJson.json.Package != null)
            {
                Elm.Package package = new(ElmJson.json.Package, ElmX_Json);
                unusedImports = Imports.FindUnused(package);
            }
            else
            {
                Writer.EmptyLine();
                Writer.WriteLine("I could not find the source directories for this project.");
                Environment.Exit(0);
            }

            if (unusedImports.Count == 0)
            {
                Writer.EmptyLine();
                Writer.WriteLine("I did not find any unused imports.");
                Environment.Exit(0);
            }


            if (options.Show && options.Pause)
            {
                Writer.EmptyLine();
                Writer.WriteLine("You asked me to show you the unused imports and then pause before deleting them. I will do that now.");
                Writer.EmptyLine();
                foreach (KeyValuePair<string, List<string>> unusedImport in unusedImports)
                {
                    Writer.WriteLine(unusedImport.Key);
                    Writer.WriteLines(unusedImport.Value);
                }

                PauseUnusedImports(unusedImports);

                Environment.Exit(0);
            }

            if (options.Show && options.Delete)
            {
                Writer.EmptyLine();
                Writer.WriteLine("You asked me to show you the unused imports and then delete them. I will do that now.");
                Writer.EmptyLine();
                foreach (KeyValuePair<string, List<string>> unusedImport in unusedImports)
                {
                    Writer.WriteLine(unusedImport.Key);
                    Writer.WriteLines("\t", unusedImport.Value);
                }

                //DeleteUnusedImports(unusedImports);

                Environment.Exit(0);
            }

            if (options.Delete)
            {
                Writer.EmptyLine();
                Writer.WriteLine("You asked me to delete the unused modules. I will do that now.");

                DeleteUnusedImports(unusedImports);

                Environment.Exit(0);
            }

            if (options.Pause)
            {
                Writer.EmptyLine();
                Writer.WriteLine("You asked me to pause before deleting the unused imports. I will do that now.");

                PauseUnusedImports(unusedImports);

                Environment.Exit(0);
            }


            if (options.Show)
            {
                Writer.EmptyLine();
                Writer.WriteLine("You asked me to show you the unused imports. I will do that now.");
                Writer.EmptyLine();

                Writer.WriteLine($"Found: {unusedImports.Count} file(s) with unused imports");
                foreach (KeyValuePair<string, List<string>> unused in unusedImports)
                {
                    Writer.EmptyLine();
                    Writer.WriteLine($"File: {unused.Key}");
                    Writer.WriteLine($"Found: {unused.Value.Count} unused imports");
                    foreach (string unusedImport in unused.Value)
                    {
                        Writer.WriteLine($"\t{unusedImport}");
                    }
                }

                Environment.Exit(0);
            }
        }

        private void DeleteUnusedImports(Dictionary<string, List<string>> unusedImports)
        {
            Writer.EmptyLine();
            Writer.WriteLine("I will now delete the unused imports.");
            Writer.EmptyLine();

            foreach (KeyValuePair<string, List<string>> unusedImport in unusedImports)
            {
                foreach (string import in unusedImport.Value)
                {
                    Writer.WriteLine($"I will delete unused import, {import}, from {unusedImport.Key}.");
                    Writer.EmptyLine();
                    Writer.WriteLine("TODO: Actually delete the import.");
                    //DeleteImport(unusedImport.Key, import);
                }
            }
        }

        private void PauseUnusedImports(Dictionary<string, List<string>> unusedImports)
        {
            Writer.EmptyLine();
            Writer.WriteLine("I will now delete the unused imports.");
            Writer.EmptyLine();

            foreach (KeyValuePair<string, List<string>> unusedImport in unusedImports)
            {
                Writer.WriteLine($"In file: {unusedImport.Key}");
                Writer.WriteLine($"I found {unusedImport.Value.Count} unused imports, and will step through them now.");
                foreach (string import in unusedImport.Value)
                {
                    MaybeDeleteImport(unusedImport.Key, import);
                }
                Writer.WriteLine($"Delete: {unusedImport.Value}, (y/n/(q)uit)");
            }
        }

        private void MaybeDeleteImport(string file, string import)
        {
            string input = Reader.ReadKey();
            if (input == "y")
            {
                Writer.WriteLine($"I will delete unused import, {import}, from {file}.");
                Writer.EmptyLine();
                Writer.WriteLine("TODO: Actually delete the import.");
                //DeleteImport(file, import);
            }
            else if (input == "n")
            {
                Writer.WriteLine($"I will not delete {import} from {file}.");
            }
            else if (input == "q")
            {
                Writer.WriteLine($"I will not delete {import} from {file}.");
                Environment.Exit(0);
            }
            else
            {
                Writer.WriteLine($"I did not understand your input. Please try again.");
                MaybeDeleteImport(file, import);
            }
        }
    }
}