namespace ElmX.Commands
{
    class OptionsParser
    {
        // Help Intro Option - set to true if the user runs only `elmx` with no arguments
        public bool HelpIntro { get; private set; }

        // No Cmd Options
        public bool Help { get; private set; }

        public bool Version { get; private set; }


        // The Command to be run
        public Cmd Cmd { get; private set; }

        // Options
        public Init.Options InitOptions { get; private set; } = new();

        public UnusedModules.Options UnusedModulesOptions { get; private set; } = new();

        public UnusedImports.Options UnusedImportsOptions { get; private set; } = new();


        // Unused Modules Options

        // Unknowns

        public string UnknownCmd { get; private set; } = "";

        public List<string> UnknownNoCmdArgs { get; private set; } = new List<string>();


        public OptionsParser(string[] args)
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
                "unused-imports" => Cmd.UnusedImports,
                _ => Cmd.Unknown,
            };
        }

        private void ParseCommandArgs(List<string> args)
        {
            switch (Cmd)
            {
                case Cmd.Init:
                    InitOptions.Parse(args);
                    break;
                case Cmd.UnusedModules:
                    UnusedModulesOptions.Parse(args);
                    break;
                case Cmd.UnusedImports:
                    UnusedImportsOptions.Parse(args);
                    break;
                case Cmd.Unknown:
                    UnknownCmd = args[0];
                    break;
            }
        }
    }

    enum Cmd
    {
        Init,
        UnusedModules,

        UnusedImports,

        Unknown
    }
}