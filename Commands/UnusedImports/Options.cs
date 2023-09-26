using ElmX.Core.Console;

namespace ElmX.Commands.UnusedImports
{
    public class Options : ElmX.Commands.Options
    {
        public bool Delete { get; private set; }

        public bool Pause { get; private set; }

        public bool Rename { get; private set; }

        public bool Show { get; private set; }

        public void Parse(List<string> args)
        {
            short counter = 0;

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
                        case "-d":
                        case "--delete":
                            Delete = true;
                            break;
                        case "-e":
                        case "--exclude":
                            ParseExcludedDirs(args, counter);
                            break;
                        case "-p":
                        case "--pause":
                            Pause = true;
                            break;
                        case "-r":
                        case "--rename":
                            Rename = true;
                            break;
                        case "-s":
                        case "--show":
                            Show = true;
                            break;

                    }
                }
                counter++;
            }
        }
    }
}