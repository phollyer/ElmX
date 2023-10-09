using ElmX.Core.Parser.Whitespace;

namespace ElmX.Core.Parser
{
    public class ModuleStatement
    {
        public string Name { get; set; } = "";

        public string Exposing { get; set; } = "";

        public List<Comment> Comments { get; set; } = new();

        public List<Newline> Newlines { get; set; } = new();

        public List<Space> Spaces { get; set; } = new();

        static public (ModuleStatement, int) Parse(int index, string content)
        {

            ModuleStatement moduleStatement = new();

            bool statementComplete = false;

            string name = "";
            bool nameFound = false;
            bool nameComplete = false;

            for (int i = index; i < content.Length; i++)
            {
                switch (content[i])
                {

                    case '{':
                        string commentContent = String.ExtractInner(('{', '}'), content[i..]);
                        string comment = "{" + commentContent + "}";
                        moduleStatement.Comments.Add(new Comment(CommentType.MultiLine, comment, i));

                        if (nameFound && !nameComplete)
                        {
                            nameComplete = true;
                            moduleStatement.Name = name;
                        }
                        i = i + comment.Length - 1;
                        break;

                    case '(':
                        string exposingContent = String.ExtractInner(('(', ')'), content[i..]);
                        string exposing = "(" + exposingContent + ")";
                        moduleStatement.Exposing = exposing;

                        statementComplete = true;
                        i = i + exposing.Length - 1;
                        break;

                    case ' ':
                        moduleStatement.Spaces.Add(new Space(i));
                        if (nameFound && !nameComplete)
                        {
                            nameComplete = true;
                            moduleStatement.Name = name;
                        }
                        break;

                    case '\n':
                        moduleStatement.Newlines.Add(new Newline(i));
                        if (nameFound && !nameComplete)
                        {
                            nameComplete = true;
                            moduleStatement.Name = name;
                        }
                        break;

                    default:
                        if (!nameFound)
                        {
                            if (Char.IsUpper(content[i]))
                            {
                                nameFound = true;
                                name += $"{content[i]}";
                            }
                        }
                        else if (nameFound && !nameComplete)
                        {
                            if (Char.IsLetterOrDigit(content[i]) || content[i] == '_' || content[i] == '.')
                            {
                                name += content[i];
                            }
                        }
                        break;
                }

                if (statementComplete)
                {
                    index = i;
                    break;
                }
            }

            return (moduleStatement, index);
        }

        public override string ToString()
        {
            string str = "";
            str += $"Name: {Name};\n";
            str += $"Exposing: {Exposing};\n";
            str += $"Comments: {Comments.Count};\n";
            str += $"Newlines: {Newlines.Count};\n";
            str += $"Spaces: {Spaces.Count};\n";

            return str;
        }

    }
}