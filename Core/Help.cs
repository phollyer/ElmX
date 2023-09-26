using ElmX.Core.Console;

namespace ElmX.Core
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

        static public void ShowInitOptions()
        {
            Writer.WriteLine("init-options");
            Writer.EmptyLine();
            Writer.WriteLine("-e, --entry-file <file>\t\t\t\tSpecify the entry file of the Elm project - an example would be 'src/Main.elm'. Not applicable to packages.");
            Writer.WriteLine("-d, --exclude-dirs <dir> <dir> <dir>...\t\tExclude the specified directories from the search.");
            Writer.WriteLine("-f, --exclude-files <file> <file> <file>...\tExclude the specified files from the search.");
            Writer.EmptyLine();
        }

        static public void ShowUnusedModulesOptions()
        {
            Writer.WriteLine("[unused-modules-options]");
            Writer.EmptyLine();
            Writer.WriteLine("-d, --delete\t\t\t\tDelete the unused modules.");
            Writer.WriteLine("-e, --exclude <dir> <dir> <dir>...\tExclude the specified directories from the search.");
            Writer.WriteLine("-p, --pause\t\t\t\tPause before each deletion, requesting confirmation before deleting a module.");
            Writer.WriteLine("-r, --rename\t\t\t\tRename the unused modules. This will add a tilde (~) to the front of the module name.");
            Writer.WriteLine("-s, --show\t\t\t\tShow the unused modules.");

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

            ShowInitOptions();

            ShowUnusedModulesOptions();
        }
    }
}