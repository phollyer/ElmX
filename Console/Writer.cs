namespace ElmX.Console
{
    static class Writer
    {
        /// <summary>
        /// Clear the console.
        /// </summary>
        static public void Clear()
        {
            System.Console.Clear();
        }

        /// <summary>
        /// Write to the console and then move to the next line.
        /// </summary>
        /// <param name="line"></param>
        static public void WriteLine(string line)
        {
            System.Console.WriteLine(line);
        }

        /// <summary>
        /// Write a list of strings to the console.
        /// </summary>
        /// <param name="lines"></param>
        static public void WriteLines(List<string> lines)
        {
            foreach (string line in lines)
            {
                System.Console.WriteLine(line);
            }
        }

        /// <summary>
        /// Write a string to the console at a specific location.
        /// </summary>
        /// <param name="txt"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        static public void WriteAt(string txt, short x, short y)
        {
            try
            {
                System.Console.SetCursorPosition(x, y);
                System.Console.Write(txt);
            }
            catch (ArgumentOutOfRangeException e)
            {
                Clear();
                System.Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// Write an empty line to the console.
        /// </summary>
        static public void EmptyLine()
        {
            WriteLine("");
        }
    }
}