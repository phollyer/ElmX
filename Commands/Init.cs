// A class that takes the Init options and creates an elmx.json file in the current directory
// that contains the entry file and the excluded directories and files.

using ElmX.Console;
using ElmX.Json;

namespace ElmX.Commands
{
    static class Init
    {
        static ElmX_Json? Json { get; set; }

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
            ElmX_Json Json = new();
            Json.Create(entryFile, excludedDirs, excludedFiles);

            Writer.WriteLine("The elmx.json file has been created.");
        }
    }
}