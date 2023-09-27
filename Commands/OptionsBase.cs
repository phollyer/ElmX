namespace ElmX.Commands
{
    public class OptionsBase
    {
        public List<string> ExcludeDirs { get; set; } = new List<string>()
            { "elm-stuff"
            , "node_modules"
            , "review"
            , "tests"
            };
        public bool ShowHelp { get; set; } = false;

        protected void ParseExcludedDirs(List<string> args, short counter)
        {
            List<string> remainder = args.Skip(counter + 1).ToList();

            foreach (string item in remainder)
            {
                if (item.StartsWith("-"))
                    break;

                ExcludeDirs.Add(item);
            }
        }
    }
}