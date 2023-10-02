using ElmX.Core;
using ElmX.Core.Console;
using ElmX.Elm;

namespace ElmX.Elm.Code
{
    public class Module
    {
        public string FilePath { get; set; } = "";

        public string Name { get; private set; } = "";

        public List<string> Exposing { get; private set; } = new();

        public List<Import> Imports { get; private set; } = new();

        public List<UnionType> UnionTypes { get; private set; } = new();

        public List<TypeAlias> TypeAliases { get; private set; } = new();

        public List<Function> Functions { get; private set; } = new();

        public string RawContent { get; private set; } = "";

        public Dictionary<int, string> Content { get; private set; } = new();

        public void Read()
        {
            Lexer lexer = new(FilePath);
            lexer.Build();
            Environment.Exit(0);
        }

        public int ParseModuleStatement()
        {
            int startLine = BypassCommentsAndWhitespace(1);
            int endLine = startLine;

            for (int i = startLine; i < Content.Count; i++)
            {
                string text = Content[i];

                if (text.StartsWith("module") && text.Contains("{-"))
                {

                }
                else if (text.StartsWith("module") && text.EndsWith(")"))
                {
                    startLine = i;
                    endLine = i;
                    break;
                }
                else if (text.StartsWith("module"))
                {
                    startLine = i;
                }
                else if (text.EndsWith(")"))
                {
                    endLine = i;
                    break;
                }
            }

            if (startLine == endLine)
            {
                ParseSingleLineModuleStatement(Content[endLine]);
            }
            else
            {
                ParseMultiLineModuleStatement(startLine, endLine);
            }

            return endLine;
        }


        private void ParseSingleLineModuleStatement(string line)
        {
            line = line.Replace("module", "");
            line = line.Trim();
            string[] parts = line.Split("exposing");

            Name = parts[0].Trim();

            string exposing = parts[1].Trim();


            Exposing = ParseExposing(parts[1]);
        }

        private void ParseMultiLineModuleStatement(int startLine, int endLine)
        {
            List<string> text = new();

            for (int i = startLine; i <= endLine; i++)
            {
                text.Add(Content[i]);
            }

            string singleLineStatement = string.Join(" ", text);

            ParseSingleLineModuleStatement(singleLineStatement);
        }

        private List<string> ParseExposing(string line)
        {
            Writer.WriteLine($"line: {line}");

            line = StripTrailingComments(line);

            Writer.WriteLine($"line: {line}");

            int startIndex = 0;
            int endIndex = 0;

            for (int i = 0; i <= line.Trim().Length; i++)
            {
                if (line[i] == '(')
                {
                    startIndex = i + 1;
                    continue;
                }
                else if (line[i] == ')')
                {
                    endIndex = i;
                    break;
                }
            }

            return
                line[startIndex..endIndex]
                    .Split(',')
                    .ToList()
                    .Select(item => item.Trim())
                    .ToList()
                    ;
        }

        public void RemoveDocumentationComments()
        {
            RawContent = Comments.RemoveDocumentationComments(RawContent);
        }

        public void ParseImports()
        {
            Imports = ParseImports(RawContent);
        }

        public void RemoveImportStatementsFromContent()
        {
            string[] lines = RawContent.Split('\n');
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
            RawContent = string.Join("\n", newLines);
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

        private string StripTrailingComments(string line)
        {
            int index = line.IndexOf("--");

            if (index > -1)
            {
                line = line[0..index];
            }

            index = line.IndexOf("{-");

            if (index > -1)
            {
                line = line[0..index];
            }

            return line;
        }

        private string StripInnerComments(string line)
        {
            int index = line.IndexOf("{-");

            if (index > -1)
            {
                for (int i = index; i < line.Length; i++)
                {
                    if (line[i] == '-' && line[i + 1] == '}')
                    {
                        line = line[0..index] + line[(i + 2)..];
                        break;
                    }
                }
            }

            return line;
        }

        private int BypassCommentsAndWhitespace(int lineNumber)
        {
            bool multilineCommentFound = false;

            int lastLine = lineNumber;

            for (int i = lineNumber; i < Content.Count; i++)
            {
                string text = Content[i];

                Writer.WriteLine($"lineNumber: {i}; text: {text}");

                if (text.StartsWith("--") || (text.StartsWith("{-") && text.EndsWith("-}")))
                {
                    continue;
                }
                else if (text.StartsWith("{-"))
                {
                    multilineCommentFound = true;
                    continue;
                }
                else if (multilineCommentFound && text.EndsWith("-}"))
                {
                    multilineCommentFound = false;
                    continue;
                }
                else if (multilineCommentFound)
                {
                    continue;
                }
                else if (text.Trim() == "")
                {
                    continue;
                }
                else
                {
                    lastLine = i;
                    break;
                }
            }

            return lastLine;
        }

        public override string ToString()
        {
            string str = "";
            str += $"Path: {FilePath}\n";

            str += $"Name: {Name};\n";

            str += $"Exposing:\n";
            foreach (string exposing in Exposing)
            {
                str += $"    {exposing};\n";
            }

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