using ElmX.Core.Console;

namespace ElmX.Commands.UnusedModules
{
    public class Options
    {
        public bool Delete { get; private set; }

        public List<string> ExcludedDirs { get; private set; } = new List<string>()
            { "elm-stuff"
            , "node_modules"
            , "review"
            , "tests"
            };

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
                        case "-d":
                        case "--delete":
                            Delete = true;
                            break;
                        case "-e":
                        case "--exclude":
                            List<string> remainder = args.Skip(counter + 1).ToList();
                            foreach (string item in remainder)
                            {
                                if (item.StartsWith("-"))
                                    break;

                                ExcludedDirs.Add(item);
                            }
                            Writer.WriteLine($"Excluding: {string.Join(", ", ExcludedDirs)}");
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