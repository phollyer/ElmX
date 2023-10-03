using ElmX.Core.Console;

namespace ElmX.Elm.Code
{
    public class Import
    {
        public string Name { get; set; } = "";

        public string As { get; set; } = "";

        public List<string> Exposing { get; set; } = new List<string>();

        public Import(string statement)
        {
            string[] parts =
                statement
                .Replace('\n', ' ')
                .Replace("import", "")
                .Trim()
                .Split(' ');

            Name = parts[0];

            for (int i = 1; i < parts.Length; i++)
            {
                switch (parts[i])
                {
                    case "as":
                        As = parts[i + 1];
                        break;
                    case "exposing":
                        int start = i + 1;
                        int end = parts.Length;

                        if (start == end)
                        {
                            ParseExposing(parts[start]);
                        }
                        else
                        {
                            ParseExposing(string.Join(" ", parts[start..end]));
                        }
                        break;
                }
            }
        }

        private void ParseExposing(string exposingParts)
        {
            exposingParts = exposingParts.Trim();
            exposingParts = exposingParts[1..^1];

            string[] parts = exposingParts.Split(',');

            foreach (string part in parts)
            {
                Exposing.Add(part.Trim());
            }
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