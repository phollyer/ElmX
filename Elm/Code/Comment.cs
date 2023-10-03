namespace ElmX.Elm.Code
{
    public class Comment
    {
        public string Text { get; set; } = "";

        public CommentType Type { get; set; } = CommentType.SingleLine;

        public int StartIndex { get; set; } = 0;

    }

    public enum CommentType
    {
        SingleLine,
        MultiLine
    }
}