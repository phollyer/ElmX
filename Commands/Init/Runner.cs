// A class that takes the Init options and creates an elmx.json file in the current directory
// that contains the entry file and the excluded directories and files.

using ElmX.Console;
using ElmX.Json;
using ElmX.Commands.Options;

namespace ElmX.Commands.Init
{
    static class Runner
    {
        static ElmX_Json? Json { get; set; }

        /// <summary>
        /// Run the init command in the current directory.
        /// </summary>
        /// <param name="entryFile">
        /// The entry file of the Elm project.
        /// </param>
        /// <param name="excludedDirs">
        /// The list of directories to exclude.
        /// </param>
        /// <param name="excludedFiles">
        /// The list of files to exclude.
        /// </param>
        public static void Run(InitOptions options)
        {
            ElmX_Json Json = new();

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
                    case "n":
                        Writer.EmptyLine();
                        Writer.WriteLine("Exiting...");
                        Environment.Exit(0);
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