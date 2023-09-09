using ElmX.Console;
using ElmX.Options;
using System.Reflection;

namespace ElmX
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser options = new(args);

            if (options.HelpIntro)
            {
                Help.ShowIntro();
                Environment.Exit(0);
            }

            if (options.Help && options.Version)
            {
                PrintVersion();

                Help.ShowIntro();
                Help.Show();
                Environment.Exit(0);
            }

            if (options.Help)
            {
                Help.ShowIntro();
                Help.Show();
                Environment.Exit(0);
            }

            if (options.Version)
            {
                PrintVersion();

                Environment.Exit(0);
            }

            if (options.UnknownNoCmdArgs.Count > 0)
            {
                Writer.EmptyLine();
                foreach (string arg in options.UnknownNoCmdArgs)
                {
                    Writer.WriteLine($"I don't know what '{arg}' means.");
                }
                Writer.WriteLine("Please use the -h or --help option to see what I need in order to run.");
                Writer.EmptyLine();
                Environment.Exit(0);
            }

            if (options.Cmd == Cmd.Init)
            {
                Init.Runner.Run(options.EntryFile, options.ExcludedDirs, options.ExcludedFiles);
                Environment.Exit(0);
            }

            if (options.Cmd == Cmd.UnusedModules)
            {
                UnusedModules.Runner.Run(options);
                Environment.Exit(0);
            }

            Writer.EmptyLine();
            Writer.WriteLine("I don't know what you want me to do. Please use the -h or --help option to see the available commands.");
        }

        static void PrintVersion()
        {
            Assembly? assembly = Assembly.GetEntryAssembly();
            AssemblyName? assemblyName = assembly?.GetName();
            Version? version = assemblyName?.Version;
            if (version is not null)
            {
                Writer.WriteLine($"v{version.ToString()}");
            }
            else
            {
                Writer.WriteLine("I don't know what version I am.");
            }
        }
    }
}
