using ElmX.Console;
using ElmX.Options;

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

            if (options.Help)
            {
                Help.ShowIntro();
                Help.Show();
                Environment.Exit(0);
            }

            if (options.Version)
            {
                Writer.WriteLine("ElmX version 1.0.0");
                Environment.Exit(0);
            }

            if (options.UnusedModules)
            {
                UnusedModules.Runner.Run(options);
            }

            Writer.EmptyLine();
            Writer.WriteLine("I don't know what you want me to do. Please use the -h or --help option to see the available commands.");
        }
    }
}
