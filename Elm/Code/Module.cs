using ElmX.Core;
using ElmX.Core.Console;

namespace ElmX.Elm.Code
{
    public class Module
    {
        public string FilePath { get; set; } = "";

        public List<Import> Imports { get; private set; } = new List<Import>();

        public List<UnionType> UnionTypes { get; private set; } = new List<UnionType>();

        public List<TypeAlias> TypeAliases { get; private set; } = new List<TypeAlias>();

        public List<Function> Functions { get; private set; } = new List<Function>();

        public string Content { get; private set; } = "";

        public void Read()
        {
            Content = File.ReadAllText(FilePath);
        }

        public void RemoveDocumentationComments()
        {
            Content = Comments.RemoveDocumentationComments(Content);
        }

        public void ParseImports()
        {
            Imports = ParseImports(Content);
        }

        public void RemoveImportStatementsFromContent()
        {
            string[] lines = Content.Split('\n');
            string[] newLines = Array.Empty<string>();

            int[] lineNumbers = GetImportLineNumbers(lines);

            if (lineNumbers.Length == 0)
            {
                return;
            }

            Writer.WriteLine($"lineNumbers: {string.Join(", ", lineNumbers)}");

            int lastLineNumberOfLastImport = GetLastLineNumberOfLastImport(lines, lineNumbers[^1]);

            if (lastLineNumberOfLastImport == -1)
            {
                lastLineNumberOfLastImport = lines.Length;
            }
            else
            {
                lineNumbers = lineNumbers.Append(lastLineNumberOfLastImport + 1).ToArray();
            }

            for (int lineNumber = 0; lineNumber < lines.Length; lineNumber++)
            {
                bool lineIsImport = false;

                for (int i = 0; i < lineNumbers.Length - 1; i++)
                {
                    int startLine = lineNumbers[i];
                    int endLine = lineNumbers[i + 1];

                    Writer.WriteLine($"lineNumber: {lineNumber}, startLine: {startLine}, endLine: {endLine}");

                    if (lineNumber >= startLine && lineNumber <= endLine)
                    {
                        lineIsImport = true;
                        break;
                    }
                }

                if (lineIsImport)
                {
                    continue;
                }
                else
                {
                    newLines = newLines.Append(lines[lineNumber]).ToArray();
                }
            }
            Content = string.Join("\n", newLines);
        }

        private List<Import> ParseImports(string content)
        {
            string[] lines = content.Split('\n');

            int[] lineNumbers = GetImportLineNumbers(lines);

            if (lineNumbers.Length == 0)
            {
                return new List<Import>();
            }

            int lastLineNumberOfLastImport = GetLastLineNumberOfLastImport(lines, lineNumbers[^1]);

            if (lastLineNumberOfLastImport == -1)
            {
                lastLineNumberOfLastImport = lines.Length;
            }
            else
            {
                lineNumbers = lineNumbers.Append(lastLineNumberOfLastImport + 1).ToArray();
            }

            for (int lineNumber = 0; lineNumber < lineNumbers.Length; lineNumber++)
            {
                if (lineNumber == lineNumbers.Length - 1)
                {
                }
                else if (lineNumbers[lineNumber] == lineNumbers[lineNumber + 1] - 1)
                {
                    string line = lines[lineNumbers[lineNumber] - 1];
                    ParseSingleLineImport(line);
                }
                else
                {
                    int startLine = lineNumbers[lineNumber] - 1;
                    int endLine = lineNumbers[lineNumber + 1] - 1;

                    string[] importLines = lines[startLine..endLine];

                    ParseImportLines(importLines);
                }
            }

            return Imports;
        }

        private int[] GetImportLineNumbers(string[] lines)
        {
            int[] lineNumbers =
                lines
                .Select((line, index) => line.StartsWith("import") || line.StartsWith("-- import") || line.StartsWith("{- import") ? index + 1 : -1)
                .Where(index => index != -1).ToArray();

            return lineNumbers;
        }
        private int GetLastLineNumberOfLastImport(string[] lines, int lineNumber)
        {
            for (int i = 1; i < lines.Length; i++)
            {
                if (lines[lineNumber + i].StartsWith(" ") || lines[lineNumber + i].StartsWith("\t"))
                {
                    continue;
                }
                else
                {
                    return lineNumber + i;
                }
            }

            return -1;
        }

        private void ParseImportLines(string[] importLines)
        {
            for (int i = 0; i < importLines.Length; i++)
            {
                importLines[i] = importLines[i].Trim();
            }
            string importLine = string.Join(" ", importLines);

            ParseSingleLineImport(importLine);
        }

        private void ParseSingleLineImport(string line)
        {
            line = Comments.Remove(line);

            line = line.Replace("import", "");
            line = line.Trim();
            string[] parts = line.Split(" ");
            Import import = new()
            {
                Name = parts[0]
            };

            for (int i = 1; i < parts.Length; i++)
            {
                switch (parts[i])
                {
                    case "as":
                        import.As = parts[i + 1];
                        break;
                    case "exposing":
                        int start = i + 1;
                        int end = parts.Length;

                        if (start == end)
                        {
                            ParseExposing(import, parts[start]);
                        }
                        else
                        {
                            ParseExposing(import, string.Join(" ", parts[start..end]));
                        }
                        break;
                }
            }

            Imports.Add(import);
        }

        private void ParseExposing(Import import, string exposingParts)
        {
            int lastParenthesisIndex = exposingParts.LastIndexOf(')');

            if (lastParenthesisIndex > -1)
            {
                exposingParts = exposingParts[0..(lastParenthesisIndex + 1)];
            }

            exposingParts = exposingParts[1..^1];

            exposingParts = exposingParts.Replace(" ", "");

            string[] parts = exposingParts.Split(',');

            foreach (string part in parts)
            {
                import.Exposing.Add(part);
            }
        }

        public override string ToString()
        {
            string str = "";
            str += $"Path: {FilePath}\n";

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