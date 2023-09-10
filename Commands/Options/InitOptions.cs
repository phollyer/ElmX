namespace ElmX.Commands.Options
{
    public class InitOptions
    {
        public string EntryFile { get; set; } = "Main.elm";

        public List<string> ExcludedDirs { get; set; } = new List<string>()
            { "elm-stuff"
            , "node_modules"
            , "review"
            , "tests"
            };

        public List<string> ExcludedFiles { get; set; } = new List<string>();
        public void Parse(List<string> args)
        {
            short index = 0;

            foreach (string arg in args)
            {
                if (arg.StartsWith("-"))
                {
                    switch (arg)
                    {
                        case "-e":
                        case "--entry-file":
                            EntryFile = args[index + 1];
                            break;
                        case "-d":
                        case "--exclude-dirs":
                            foreach (string dir in args.Skip(index + 1))
                            {
                                if (dir.StartsWith("-"))
                                    break;

                                ExcludedDirs.Add(dir);
                            }
                            break;
                        case "-f":
                        case "--exclude-files":
                            foreach (string item in args.Skip(index + 1))
                            {
                                if (item.StartsWith("-"))
                                    break;

                                ExcludedFiles.Add(item);
                            }
                            break;
                    }
                }

                index++;
            }
        }
    }
}