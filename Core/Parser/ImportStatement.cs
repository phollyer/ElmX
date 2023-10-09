using ElmX.Core.Console;
using ElmX.Core.Parser.Whitespace;

namespace ElmX.Core.Parser
{
    public class ImportStatement
    {
        public string Name { get; set; } = "";

        public string As { get; set; } = "";

        public string Exposing { get; set; } = "";

        public List<Comment> Comments { get; set; } = new();

        public List<Newline> Newlines { get; set; } = new();

        public List<Space> Spaces { get; set; } = new();

        static public (ImportStatement, int) Parse(int index, string content)
        {
            ImportStatement importStatement = new();

            int startOfNextStatement = FindEndOfStatement(index, content);

            string statementContent = content[index..startOfNextStatement].TrimEnd();

            int endIndex = index + statementContent.Length;

            Writer.WriteLine($"Statement Content: {content[index..endIndex]}");

            bool extractingName = false;
            bool nameComplete = false;

            bool extractingAs = false;
            bool asComplete = false;

            for (int i = index; i < endIndex; i++)
            {
                char c = content[i];

                switch (c)
                {
                    case '{':
                        string commentContent = String.ExtractInner(('{', '}'), content[i..]);
                        string comment = "{" + commentContent + "}";
                        importStatement.Comments.Add(new Comment(CommentType.MultiLine, comment, i));
                        i = i + comment.Length - 1;
                        break;

                    case '(':
                        string exposingContent = String.ExtractInner(('(', ')'), content[i..]);
                        string exposing = "(" + exposingContent + ")";
                        importStatement.Exposing = exposing;
                        i = i + exposing.Length - 1;
                        break;

                    case ' ':
                        importStatement.Spaces.Add(new Space(i));

                        if (extractingName && !nameComplete)
                        {
                            nameComplete = true;
                            extractingName = false;
                        }
                        else if (extractingAs && !asComplete)
                        {
                            asComplete = true;
                            extractingAs = false;
                        }
                        break;

                    case '\n':
                        importStatement.Newlines.Add(new Newline(i));

                        if (extractingName && !nameComplete)
                        {
                            nameComplete = true;
                            extractingName = false;
                        }
                        else if (extractingAs && !asComplete)
                        {
                            asComplete = true;
                            extractingAs = false;
                        }
                        break;


                    default:
                        if (Char.IsUpper(c) && !extractingName && !nameComplete)
                        {
                            extractingName = true;
                            importStatement.Name += c;
                        }
                        else if (extractingName && !nameComplete)
                        {
                            importStatement.Name += c;
                        }
                        else if (Char.IsUpper(c) && !extractingAs && !asComplete)
                        {
                            extractingAs = true;
                            importStatement.As += c;
                        }
                        else if (extractingAs && !asComplete)
                        {
                            importStatement.As += c;
                        }
                        break;
                }

            }

            Writer.WriteLine(importStatement.ToString());

            return (importStatement, endIndex);
        }

        static private int FindEndOfStatement(int index, string content)
        {
            int endIndex = index;
            for (int i = index; i < content.Length; i++)
            {
                char c = content[i];

                switch (c)
                {
                    case '{':
                        string commentContent = String.ExtractInner(('{', '}'), content[i..]);
                        string comment = "{" + commentContent + "}";
                        i = i + comment.Length - 1;
                        break;

                    case '-':
                        if (content[i + 1] == '-')
                        {
                            endIndex = i - 1;
                        }
                        break;

                    case '\n':
                        if (Char.IsLetter(content[i + 1]))
                        {
                            endIndex = i;
                            break;
                        }
                        break;
                }

                if (endIndex != index)
                {
                    break;
                }
            }

            return endIndex;
        }

        public override string ToString()
        {
            string str = "";
            str += $"Name: {Name};\n";
            str += $"As: {As};\n";
            str += $"Exposing: {Exposing};\n";
            str += $"Comments: {Comments.Count};\n";
            str += $"Newlines: {Newlines.Count};\n";
            str += $"Spaces: {Spaces.Count};\n";

            return str;
        }
    }
}