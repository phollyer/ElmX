using ElmX.Core;

namespace ElmX.Elm.Code
{
    public class Module
    {
        public string Path { get; private set; } = "";

        public List<Import> Imports { get; private set; } = new List<Import>();

        public List<UnionType> UnionTypes { get; private set; } = new List<UnionType>();

        public List<TypeAlias> TypeAliases { get; private set; } = new List<TypeAlias>();

        public List<Function> Functions { get; private set; } = new List<Function>();

        public string Content { get; private set; } = "";

        public Module() { }

        public Module(string path)
        {
            Path = path;
        }

        public void ParseImports()
        {
            Content = File.ReadAllText(Path);

            Content = Comments.RemoveDocumentationComments(Content);

            Imports = new Imports().Parse(Content);
        }

        public override string ToString()
        {
            string str = "";

            str += $"Path: {Path}\n";

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