using ElmX.Core.Console;
using ElmX.Core.Parser;

namespace ElmX.Elm.Code
{
    public class Import
    {
        public string Name { get; set; } = "";

        public string As { get; set; } = "";

        public List<string> Exposing { get; set; } = new List<string>();

        public Import(ImportStatement statement)
        {
            Name = statement.Name;
            As = statement.As;
            Exposing = statement.Exposing;
        }

        public override string ToString()
        {
            string exposing = "";

            if (Exposing.Count > 0)
            {
                exposing = $" exposing ({string.Join(", ", Exposing)})";
            }

            if (As != "")
            {
                return $"import {Name} as {As}{exposing}";
            }
            else
            {
                return $"import {Name}{exposing}";
            }
        }
    }
}