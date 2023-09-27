using ElmX.Core;
using ElmX.Core.Console;
using ElmX.Elm;

namespace ElmX.Commands
{
    class RunnerBase
    {
        protected Elm.Json ElmJson = new();

        protected Core.Json ElmX_Json = new();

        public RunnerBase()
        {
            ElmJson.Read();

            if (ElmJson.json == null)
            {
                Writer.EmptyLine();
                Writer.WriteLine("I could not find an elm.json file in the current directory.");
                Environment.Exit(0);
            }

            ElmX_Json.Read();

            if (ElmX_Json.AppJson == null && ElmX_Json.PkgJson == null)
            {
                Writer.EmptyLine();
                Writer.WriteLine("I could not find an elmx.json file in the current directory. Please run the init command first.");
                Environment.Exit(0);
            }
        }
    }
}