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