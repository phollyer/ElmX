namespace ElmX.Elm.Code
{
    public class UnionType
    {
        public string Name { get; set; } = "";
        public List<Constructor> Constructors { get; set; } = new();

        public UnionType(string statement)
        {
            string body = ParseName(statement);
            ParseConstructors(body);
        }

        private string ParseName(string statement)
        {
            string[] parts = statement.Split('=');

            Name =
                parts[0]
                .Replace("type", "")
                .Trim()
                ;

            return parts[1];
        }

        private void ParseConstructors(string body)
        {
            string[] parts = body.Split('|');

            foreach (string part in parts)
            {
                string[] constructorParts = part.Split(' ');

                Constructor constructor = new();

                constructor.Name = constructorParts[0];

                string fields = string.Join(' ', constructorParts);

                Constructors.Add(constructor);
            }
        }

        public override string ToString()
        {
            string result = "";

            result += $"type {Name} = \n";

            foreach (Constructor constructor in Constructors)
            {
                result += $"    | {constructor.Name} {string.Join(' ', constructor.Fields)} \n";
            }

            return result;
        }
    }

    public class Constructor
    {
        public string Name { get; set; } = "";
        public List<string> Fields { get; set; } = new();
    }
}