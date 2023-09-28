namespace ElmX.Core
{
    static class Path
    {
        static public string FromDotNotation(string filepath)
        {
            return filepath.Replace('.', System.IO.Path.DirectorySeparatorChar) + ".elm";
        }
    }
}