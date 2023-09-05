// A class that parses an elmx.json files.

using ElmX.Console;

namespace ElmX.Json
{
    static class ElmxJson
    {
        /// <summary>
        /// The entry point for the Elm application.
        /// </summary>
        /// <value>
        /// The default value is "Main.elm".
        /// </value>
        static public string Entry = "Main.elm";

        /// <summary>
        /// Read the elmx.json file.
        /// </summary>
        /// <param name="dir">
        /// The directory that contains the elmx.json file.
        /// </param>
        static public void Read(string dir)
        {
            try
            {
                string path = Path.Combine(dir, "elmx.json");
                var lines = from line in File.ReadLines(path)
                            where line.Contains("entry")
                            select new
                            {
                                Line = line.Replace("\"entry\":", "").Trim().Replace("\"", ""),
                            };

                Entry = lines.First().Line;
            }
            catch (FileNotFoundException ex)
            {
                Writer.WriteLine($"I could not find an {ex.FileName} therefore I am assuming {Entry} is your entry point.");
            }
        }
    }
}