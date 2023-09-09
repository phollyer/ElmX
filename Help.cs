// A class that writes the Help text for all the available Options to the console

using ElmX.Console;

namespace ElmX
{
    static class Help
    {
        /// <summary>
        /// Show the introductory help text.
        /// </summary>
        static public void ShowIntro()
        {
            Writer.EmptyLine();
            Writer.WriteLine("ElmX Help");

            Writer.EmptyLine();
            Writer.WriteLine("Usage: elmx [options]");

            Writer.EmptyLine();
            Writer.WriteLine("-h, --help\tShow command line help.");
            Writer.WriteLine("-v, --version\tDisplay the current version being used.");
            Writer.EmptyLine();
            Writer.WriteLine("Usage: elmx command [command-options]");
            Writer.EmptyLine();
        }

        /// <summary>
        /// Show the help text for the unused-modules command.
        /// </summary>
        static public void Show()
        {
            Writer.WriteLine("Commands:");
            Writer.EmptyLine();
            Writer.WriteLine("init\t\tCreate an elmx.json file in the current directory.");
            Writer.WriteLine("unused-modules\tFind and remove unused modules in an Elm project.");
            Writer.EmptyLine();

            Writer.WriteLine("init-options");
            Writer.EmptyLine();
            Writer.WriteLine("-e, --entry-file <file>\t\t\t\tSpecify the entry file of the Elm project.");
            Writer.WriteLine("-d, --exclude-dirs <dir> <dir> <dir>...\t\tExclude the specified directories from the search.");
            Writer.WriteLine("-f, --exclude-files <file> <file> <file>...\tExclude the specified files from the search.");
            Writer.EmptyLine();

            Writer.WriteLine("[unused-modules-options]");
            Writer.EmptyLine();
            Writer.WriteLine("-D, --dir <dir>\t\t\t\tSpecify the directory to search. If no directory is specified, the current directory is used.");
            Writer.WriteLine("-d, --delete\t\t\t\tDelete the unused modules.");
            Writer.WriteLine("-e, --exclude <dir> <dir> <dir>...\tExclude the specified directories from the search.");
            Writer.WriteLine("-p, --pause\t\t\t\tPause before each deletion, requesting confirmation before deleting a module.");
            Writer.WriteLine("-r, --rename\t\t\t\tRename the unused modules. This will add a tilde (~) to the front of the module name.");
            Writer.WriteLine("-s, --show\t\t\t\tShow the unused modules.");

            Writer.EmptyLine();
        }
    }
}