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
                Writer.WriteLine("You asked me to find the unused modules, but you didn't tell me what to do with them. Please use the -h or --help option to see what your options are.");
                Environment.Exit(0);
            }

            Writer.Clear();
            Writer.WriteLine("I will now search for unused modules.");

            if (ElmJson.json.projectType == ProjectType.Application && ElmJson.json.Application != null)
            {
                Elm.Application application = new(ElmJson.json.Application, ElmX_Json);
                unusedImports = application.FindUnusedImports();
            }
            else if (ElmJson.json.projectType == ProjectType.Package && ElmJson.json.Package != null)
            {
                Elm.Package package = new(ElmJson.json.Package, ElmX_Json);
                //unusedImports = package.FindUnusedImports();
            }
            else
            {
                Writer.EmptyLine();
                Writer.WriteLine("I could not find the source directories for this project.");
                Environment.Exit(0);
            }



        }
    }
}