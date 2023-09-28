namespace ElmX.Elm.Code
{
    public class UnionType
    {
        public string Name { get; set; } = "";
        public List<Constructor> Constructors { get; set; } = new();
    }

    public class Constructor
    {
        public string Name { get; set; } = "";
        public List<string> Fields { get; set; } = new();
    }
}