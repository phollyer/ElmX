using ElmX.Core;
using ElmX.Core.Console;
using ElmX.Elm;

namespace ElmX.Commands.UnusedImports
{
    class Runner
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

            unusedImports = Run();

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

            if (options.Delete)
            {
                Writer.EmptyLine();
                Writer.WriteLine("You asked me to delete the unused modules. I will do that now.");

                DeleteUnusedImports(unusedImports);

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

        static private Dictionary<string, List<string>> Run()
        {
            if (ElmJson.json.projectType == ProjectType.Application && ElmJson.json.Application != null)
            {
                Elm.Application app = new(ElmJson.json.Application, ElmX_Json);

                Dictionary<string, List<string>> unused = app.FindUnusedImports();

                return unused;

            }
            else if (ElmJson.json.projectType == ProjectType.Package && ElmJson.json.Package != null)
            {
                Elm.Package pkg = new(ElmJson.json.Package, ElmX_Json);

                Dictionary<string, List<string>> unused = pkg.FindUnusedImports();

                return unused;
            }
            else
            {
                return new();
            }
        }

        static private void WriteSummary(Dictionary<string, List<string>> unused, List<string> sourceDirs, List<string> excludeDirs, List<string> excludeFiles, List<string> modulePaths, List<string> allFiles)
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

            Writer.WriteAt($"Found: {unused.Count} files with unused imports", 0, lineNumberToWriteAt);
            Writer.EmptyLine();
        }

        static private void DeleteUnusedImports(Dictionary<string, List<string>> unusedImports)
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

        static private void PauseUnusedImports(Dictionary<string, List<string>> unusedImports)
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

        static private void MaybeDeleteImport(string file, string import)
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