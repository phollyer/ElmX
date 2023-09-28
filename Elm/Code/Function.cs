namespace ElmX.Elm.Code
{
    public class Function
    {
        public string Name { get; set; } = "";
        public List<Parameter> Parameters { get; set; } = new();

        public string Body { get; set; } = "";
        public string ReturnType { get; set; } = "";
    }

    public class Parameter
    {
        public string Name { get; set; } = "";
        public string Type { get; set; } = "";
    }
}