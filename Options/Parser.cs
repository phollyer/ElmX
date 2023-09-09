// A Class that parses and records the command line arguments as detailed in Help.cs

using ElmX.Console;

namespace ElmX.Options
{
    class Parser
    {
        // Help Intro Option - set to true if the user runs only `elmx` with no arguments
        public bool HelpIntro { get; private set; }

        // No Cmd Options
        public bool Help { get; private set; }

        public bool Version { get; private set; }


        // The Command to be run
        public Cmd Cmd { get; private set; }

        // Init Options
        public string EntryFile { get; private set; } = "Main.elm";

        public List<string> ExcludedDirs { get; private set; } = new List<string>()
            { "elm-stuff"
            , "node_modules"
            , "review"
            , "tests"
            };

        public List<string> ExcludedFiles { get; private set; } = new List<string>();


        // Unused Modules Options

        public bool Delete { get; private set; }

        public bool Pause { get; private set; }

        public bool Rename { get; private set; }

        public bool Show { get; private set; }

        // Unknowns

        public string UnknownCmd { get; private set; } = "";

        public List<string> UnknownNoCmdArgs { get; private set; } = new List<string>();

        public string Dir { get; private set; } = ".";

        public Parser(string[] args)
        {
            if (args.Length == 0)
            {
                HelpIntro = true;
            }
            else if (args[0].StartsWith("-"))
            {
                ParseNoCmdArgs(args);
            }
            else
            {
                ParseCmd(args[0]);

                if (Cmd != Cmd.Unknown)
                {
                    ParseCommandArgs(args.Skip(1).ToList());
                }
            }
        }

        private void ParseNoCmdArgs(string[] args)
        {
            foreach (string arg in args)
            {
                switch (arg)
                {
                    case "-h":
                    case "--help":
                        Help = true;
                        break;
                    case "-v":
                    case "--version":
                        Version = true;
                        break;
                    case "-vh":
                    case "-hv":
                        Help = true;
                        Version = true;
                        break;
                    default:
                        UnknownNoCmdArgs.Add(arg);
                        break;
                }
            }
        }

        private void ParseCmd(string cmd)
        {
            Cmd = cmd switch
            {
                "init" => Cmd.Init,
                "unused-modules" => Cmd.UnusedModules,
                _ => Cmd.Unknown,
            };
        }

        private void ParseCommandArgs(List<string> args)
        {
            switch (Cmd)
            {
                case Cmd.Init:
                    ParseInitOptions(args);
                    break;
                case Cmd.UnusedModules:
                    ParseUnusedModulesOptions(args);
                    break;
                case Cmd.Unknown:
                    UnknownCmd = args[0];
                    break;
            }
        }

        private void ParseInitOptions(List<string> args)
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
        private void ParseUnusedModulesOptions(List<string> args)
        {
            short counter = 0;

            foreach (string arg in args)
            {
                if (arg.StartsWith("-"))
                {
                    switch (arg)
                    {
                        case "-D":
                        case "--dir":
                            Dir = args[counter + 1];
                            break;
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

    enum Cmd
    {
        Init,
        UnusedModules,

        Unknown
    }

    enum NoCmdOptions
    {
        Help,
        Version,

        Unknown
    }

    enum InitOptions
    {
        // The starting point for an Elm project, usually Main.elm, but may be changed by the user.
        EntryFile,

        // Directories to exclude from the search. 
        //  The defaults are 'elm-stuff', 'node_modules', 'review', and 'tests'.
        ExcludedDirs,

        // Files to exclude from the search.
        // This is useful if the user is working on one or more modules that are not yet used in the project.
        ExludedFiles
    }
}