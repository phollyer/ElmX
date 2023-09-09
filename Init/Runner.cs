// A class that takes the Init options and creates an elmx.json file in the current directory
// that contains the entry file and the excluded directories and files.

using ElmX.Console;
using System.Text.Json;

namespace ElmX.Init
{
    class Runner
    {
        /// <summary>
        /// Create the elmx.json file.
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
        public static void Run(string entryFile, List<string> excludedDirs, List<string> excludedFiles)
        {
            if (File.Exists("elmx.json"))
            {
                Writer.WriteLine("The elmx.json file already exists.");
                Environment.Exit(0);
            }

            var json = new
            {
                entryFile = entryFile,
                excludedDirs = excludedDirs,
                excludedFiles = excludedFiles,
            };

            string jsonStr = JsonSerializer.Serialize(json, new JsonSerializerOptions { WriteIndented = true });

            File.WriteAllText("elmx.json", jsonStr);

            Writer.WriteLine("The elmx.json file has been created.");
        }
    }
}