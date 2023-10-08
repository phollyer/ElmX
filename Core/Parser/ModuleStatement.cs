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