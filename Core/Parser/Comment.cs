namespace ElmX.Core.Parser
{
    public class Comment
    {
        CommentType Type { get; set; }

        string Content { get; set; } = "";

        int Index { get; set; }

        public Comment(CommentType type, string content, int index)
        {
            Type = type;
            Content = content;
            Index = index;
        }
        static public (Comment, int)? Parse(string token, int index, string content)
        {
            string comment;
            int startOfComment = index - 1;
            int endOfComment;

            switch (token)
            {
                case "--":

                    endOfComment = content.IndexOf('\n', index);

                    comment = content[startOfComment..endOfComment];

                    return (new Comment(CommentType.SingleLine, comment, startOfComment), endOfComment);

                case "{-":
                    comment = "{" + String.ExtractInner(('{', '}'), content[startOfComment..]) + "}";

                    endOfComment = index + comment.Length - 1;

                    CommentType type = CommentType.MultiLine;

                    if (comment.StartsWith("{-|"))
                    {
                        type = CommentType.Documentation;
                    }

                    return (new Comment(type, comment, startOfComment), endOfComment);

                default:
                    return null;
            }
        }

        public override string ToString()
        {
            return $"Type: {Type};\nIndex: {Index};\nContent: ->{Content}<-;\n";
        }
    }

    public enum CommentType
    {
        SingleLine,

        MultiLine,

        Documentation
    }
}