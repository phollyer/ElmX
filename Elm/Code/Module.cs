using ElmX.Core;
using ElmX.Core.Console;
using ElmX.Elm;

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

        public Lexer? Lexer { get; private set; }

        public Dictionary<int, string> Content { get; private set; } = new();

        public Module(string filePath)
        {
            FilePath = filePath;

            if (File.Exists(FilePath))
            {
                Lexer = new(FilePath);
                Lexer.Lex();

                ParseModuleStatement();
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

        public void ParseModuleStatement()
        {
            Token? token = Lexer?.GetModuleStatement();

            if (token is not null)
            {
                string line = token.Value.Replace('\n', ' ');

                line = line.Replace("module", "");
                line = line.Trim();
                string[] parts = line.Split("exposing");

                Name = parts[0].Trim();

                string exposing = parts[1].Trim();

                Exposing = ParseExposing(parts[1]);

            }
        }
        private List<string> ParseExposing(string line)
        {
            int startIndex = 0;
            int endIndex = 0;

            for (int i = 0; i <= line.Trim().Length; i++)
            {
                if (line[i] == '(')
                {
                    startIndex = i + 1;
                    continue;
                }
                else if (line[i] == ')')
                {
                    endIndex = i;
                    break;
                }
            }

            return
                line[startIndex..endIndex]
                    .Split(',')
                    .ToList()
                    .Select(item => item.Trim())
                    .ToList()
                    ;
        }

        public void ParseImports()
        {
            List<Token> tokens = Lexer?.GetImportStatements() ?? new();

            foreach (Token token in tokens)
            {
                Import Import = new(token.Value);
                Imports.Add(Import);
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

            //str += $"Union Types:\n";
            //foreach (UnionType unionType in UnionTypes)
            //{
            //str += $"    {unionType.Name}\n";
            //}
            //
            //str += $"Type Aliases:\n";
            //foreach (TypeAlias typeAlias in TypeAliases)
            //{
            //str += $"    {typeAlias.Name}\n";
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