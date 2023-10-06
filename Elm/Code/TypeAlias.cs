using ElmX.Core;
using ElmX.Core.Console;

namespace ElmX.Elm.Code
{
    public class TypeAlias
    {
        public string Name { get; set; } = "";

        // Represents an Elm type alias of the form:
        //
        // type alias Something =
        //     String
        public string? Type { get; set; }


        // Represents an Elm type alias of the form:
        //
        // type alias Something =
        //     String -> Int -> Bool ...
        public List<string>? TypeParameters { get; set; }

        // Represents an Elm type alias of the form:
        //
        // type alias Something =
        //     { field1 : String
        //     , field2 : Int
        //     , field3 : Bool
        //     ...  
        //     }
        public Dictionary<string, string>? Fields { get; set; }

        public TypeAlias(string statement)
        {
            string body = ParseName(statement);
            ParseFields(body);
        }

        private string ParseName(string statement)
        {
            string[] parts = statement.Split('=');

            Name =
                parts[0]
                .Replace("type", "")
                .Replace("alias", "")
                .Trim()
                ;

            Writer.WriteLine(Name);

            return parts[1];
        }

        private void ParseFields(string body)
        {
            if (body.Contains('{'))
            {
                ParseRecordFields(body);
            }
            else if (body.Contains("->"))
            {
                ParseTypeParameters(body);
            }
            else
            {
                ParseType(body);
            }
        }

        private void ParseType(string body)
        {
            Type = body.Trim();
        }

        private void ParseTypeParameters(string body)
        {
            TypeParameters = new();

            body = body.Trim();

            string[] parts = body.Split("->");

            foreach (string part in parts)
            {
                TypeParameters.Add(part.Trim());
            }
        }

        private void ParseRecordFields(string body)
        {
            Fields = new();

            body = body.Trim();
            body = body[1..^1];

            string value = body.Trim();

            for (int index = 0; index < value.Length; index++)
            {
                int seperatorIndex = value.IndexOf(':', index);

                if (seperatorIndex == -1)
                {
                    break;
                }

                string field =
                    value[index..seperatorIndex]
                    .Replace(',', ' ')
                    .Trim()
                    ;

                (string type, int endIndex) = ParseFieldType(value[seperatorIndex..]);

                Fields.Add(field, type);

                index = endIndex + seperatorIndex;
            }
        }

        private (string, int) ParseFieldType(string value)
        {
            int index = 0;
            string type = "";

            for (var i = 0; i < value.Length; i++)
            {
                if (Char.IsLetter(value[i]) || value[i] == ' ' || value[i] == '.' || value[i] == '_' || value[i] == '-' || value[i] == '>')
                {
                    type += value[i];
                }
                else if (value[i] == '(')
                {
                    string extracted = Core.String.ExtractInner(('(', ')'), value[i..]);

                    type += "(" + extracted + ")";

                    i = type.Length;

                    index = i;
                }
                else if (value[i] == '{')
                {
                    string extracted = Core.String.ExtractInner(('{', '}'), value[i..]);

                    type += "{" + extracted + "}";
                    i = type.Length;
                }
                else if (value[i] == ',')
                {
                    index = i;
                    break;
                }

                index = i;
            }

            return (type, index);
        }


        public override string ToString()
        {
            string output = "";

            if (Type is not null)
            {
                output += $"type alias {Name} =\n\t{Type}";
                return output;
            }
            else if (TypeParameters is not null)
            {
                output += $"type alias {Name} =\n\t{string.Join(" -> ", TypeParameters)}";
                return output;
            }
            else if (Fields is null)
            {
                return "";
            }

            output += $"type alias {Name} =\n";
            output += "\t{ ";

            List<string> fields = new();

            foreach (var field in Fields)
            {
                fields.Add($"{field.Key.Trim()} : {field.Value.Trim()}");
            }

            output += string.Join("\n \t, ", fields);

            if (fields.Count == 1)
            {
                output += " }";
            }
            else
            {
                output += "\n\t}";
            }

            return output;
        }
    }
}