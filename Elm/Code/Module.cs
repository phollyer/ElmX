using ElmX.Core.Parser;
using ElmX.Core.Console;

namespace ElmX.Elm.Code
{
    public class Module
    {
        public string FilePath { get; set; } = "";

        public string Name { get; private set; } = "";

        public List<string> Exposing { get; private set; } = new();

        public List<Comment> Comments { get; private set; } = new();

        public List<Import> Imports { get; private set; } = new();

        public List<UnionType> UnionTypes { get; private set; } = new();

        public List<TypeAlias> TypeAliases { get; private set; } = new();

        public List<Function> Functions { get; private set; } = new();

        public string RawContent { get; set; } = "";

        public Core.Parser.Parser Parser { get; private set; }

        public Dictionary<int, string> Content { get; private set; } = new();

        public Module(string filePath)
        {
            FilePath = filePath;

            if (File.Exists(FilePath))
            {
                Parser = new(FilePath);

                RawContent = Parser.Content;

                Parser.Parse();

                Environment.Exit(0);
            }
            else
            {
                Writer.EmptyLine();
                Writer.WriteLine($"File Not Found:\t{FilePath}");
                Writer.EmptyLine();
                Writer.WriteLine("Exiting...");
                Writer.EmptyLine();
                Environment.Exit(0);
            }
        }
        public override string ToString()
        {
            string str = "";
            str += $"Path: {FilePath}\n";

            str += $"Name: {Name};\n";

            str += $"Exposing:\n";
            foreach (string exposing in Exposing)
            {
                str += $"    {exposing};\n";
            }

            str += $"Imports:\n";
            foreach (Import import in Imports)
            {
                str += $"    {import.ToString()}\n";
            }

            str += $"Type Aliases:\n";
            foreach (TypeAlias typeAlias in TypeAliases)
            {
                str += $"    {typeAlias.ToString()}\n";
            }

            //str += $"Union Types:\n";
            //foreach (UnionType unionType in UnionTypes)
            //{
            //str += $"    {unionType.Name}\n";
            //}
            //
            //str += $"Functions:\n";
            //foreach (Function function in Functions)
            //{
            //str += $"    {function.Name}\n";
            //}

            return str;
        }
    }
}