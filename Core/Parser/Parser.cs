using ElmX.Core.Console;
using ElmX.Core.Parser.Whitespace;

namespace ElmX.Core.Parser
{
    public class Parser
    {

        private readonly List<string> Tokens = new()
        {
            "--",
            "{-",
            "module",
            "import"
        };

        public List<Space> Spaces { get; set; } = new();

        public List<Newline> Newlines { get; set; } = new();

        public List<Comment> Comments { get; set; } = new();

        public ModuleStatement? ModuleStatement { get; set; }

        public List<ImportStatement> ImportStatements { get; set; } = new();

        public string Content { get; set; } = "";

        public Parser(string filePath)
        {
            if (System.IO.File.Exists(filePath))
            {
                Content = System.IO.File.ReadAllText(filePath).Trim();
            }
            else
            {
                Writer.WriteLine($"ElmX.Core.Parser:\tThe file '{filePath}' does not exist.");
                Environment.Exit(0);
            }
        }

        public void Parse()
        {
            int pointer = 0;

            while (pointer < Content.Length)
            {
                pointer = Parse(pointer);
            }
        }

        private int Parse(int pointer)
        {
            string token = "";

            for (int index = pointer; index < Content.Length; index++)
            {
                if (Content[index] == '\n')
                {
                    Newlines.Add(new Newline(index));
                    return index + 1;
                }

                if (Content[index] == ' ')
                {
                    Spaces.Add(new Space(index));
                    return index + 1;
                }

                token += Content[index];

                if (Tokens.Contains(token))
                {
                    return ParseToken(token, index);
                }
            }
            return Content.Length;
        }
        private int ParseToken(string token, int index)
        {
            int endIndex = 0;

            Char nextChar = Content[index + 1];

            switch (token)
            {
                case "--":
                case "{-":
                    (Comment comment, int commentEndIndex)? commentResult = Comment.Parse(token, index, Content);

                    if (commentResult != null)
                    {
                        Comments.Add(commentResult.Value.comment);

                        endIndex = commentResult.Value.commentEndIndex;
                    }

                    break;

                case "module":
                    if (nextChar == ' ' || nextChar == '\n' || nextChar == '{')
                    {
                        (ModuleStatement moduleStatement, int moduleStatementEndIndex) = ModuleStatement.Parse(index + 1, Content);

                        ModuleStatement = moduleStatement;
                        endIndex = moduleStatementEndIndex;
                    }
                    break;

                case "import":
                    if (nextChar == ' ' || nextChar == '\n' || nextChar == '{')
                    {
                        (ImportStatement importStatement, int importStatementEndIndex) = ImportStatement.Parse(index + 1, Content);
                        ImportStatements.Add(importStatement);
                        endIndex = importStatementEndIndex;
                    }
                    break;

                default:
                    endIndex = index + 1;
                    break;
            }

            return endIndex;
        }
    }
}