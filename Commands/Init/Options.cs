namespace ElmX.Commands.Init
{
    public class Options : ElmX.Commands.OptionsBase
    {
        public string EntryFile { get; set; } = "src/Main.elm";

        public List<string> ExcludeFiles { get; set; } = new List<string>();
        public void Parse(List<string> args)
        {
            short index = 0;

            foreach (string arg in args)
            {
                if (arg.StartsWith("-"))
                {
                    switch (arg)
                    {
                        case "-h":
                        case "--help":
                            ShowHelp = true;
                            break;

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

                                ExcludeDirs.Add(dir);
                            }
                            break;
                        case "-f":
                        case "--exclude-files":
                            foreach (string item in args.Skip(index + 1))
                            {
                                if (item.StartsWith("-"))
                                    break;

                                ExcludeFiles.Add(item);
                            }
                            break;
                    }
                }

                index++;
            }
        }
    }
}