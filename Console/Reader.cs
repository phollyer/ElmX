namespace ElmX.Console
{
    static class Reader
    {
        static ConsoleKeyInfo keyInfo;

        /// <summary>
        /// Wait for the user to press a key.
        /// </summary>
        /// <returns>
        /// The key that was pressed.
        /// </returns>
        static public string ReadKey()
        {
            keyInfo = System.Console.ReadKey();

            return keyInfo.KeyChar.ToString();
        }
    }
}