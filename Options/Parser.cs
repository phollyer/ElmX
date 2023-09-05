// A Class that parses and records the command line arguments as detailed in Help.cs

using System;
using System.Collections.Generic;
using System.Linq;
using ElmX.Console;

namespace ElmX.Options
{
    class Parser
    {
        public bool Delete { get; private set; }

        public bool Help { get; private set; }

        public bool HelpIntro { get; private set; }

        public bool Pause { get; private set; }

        public bool Rename { get; private set; }

        public bool Show { get; private set; }

        public bool UnusedModules { get; private set; }

        public bool Version { get; private set; }

        public string Dir { get; private set; } = ".";

        public List<string> ExcludedDirs { get; private set; } = new List<string>();

        public Parser(string[] args)
        {
            if (args.Length == 0)
            {
                HelpIntro = true;
            }
            else
            {
                ParseArgs(args);
            }
        }

        private void ParseArgs(string[] args)
        {
            switch (args[0])
            {
                case "unused-modules":
                    UnusedModules = true;
                    ParseUnusedModulesOptions(args.Skip(1).ToList());
                    break;
                case "-h":
                case "--help":
                    Help = true;
                    break;
                default:
                    break;
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
}