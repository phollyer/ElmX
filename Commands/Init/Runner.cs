// A class that takes the Init options and creates an elmx.json file in the current directory
// that contains the entry file and the excluded directories and files.

using ElmX.Core.Console;
using ElmX.Core;

namespace ElmX.Commands.Init
{
    static class Runner
    {
        static Json? Json { get; set; }

        /// <summary>
        /// Run the init command in the current directory.
        /// </summary>
        /// <param name="options">
        /// The options supplied by the user on the command line.
        /// </param>
        public static void Run(Options options)
        {
            Json Json = new();

            if (Json.Exists)
            {
                Writer.EmptyLine();
                Writer.WriteLine("The elmx.json file already exists.");
                Writer.EmptyLine();

                Writer.Write("Do you want to overwrite the current elmx.json file? (y/n) ");

                string key = Reader.ReadKey();

                switch (key)
                {
                    case "y":
                        File.Delete("elmx.json");

                        Writer.EmptyLine();

                        Json.Create(options);

                        Writer.EmptyLine();
                        Writer.WriteLine("A fresh elmx.json has been created.");
                        break;
                    default:
                        Writer.EmptyLine();
                        Writer.WriteLine("Exiting...");
                        Environment.Exit(0);
                        break;
                }
            }
            else
            {
                Json.Create(options);

                Writer.EmptyLine();
                Writer.WriteLine("elmx.json has been created.");
            }
        }
    }
}