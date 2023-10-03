using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElmX.Core.Console;
using ElmX.Elm.Tokens;

namespace ElmX.Elm
{
    public class Lexer
    {

        private string Content { get; set; } = "";

        private List<Token> Tokens { get; set; } = new List<Token>();

        private bool ModuleStatementFound { get; set; } = false;

        private bool AllImportsFound { get; set; } = false;

        public Lexer(string filePath)
        {
            if (System.IO.File.Exists(filePath))
            {
                Content = System.IO.File.ReadAllText(filePath).Trim();
            }
            else
            {
                Writer.WriteLine($"ElmX.Elm.Lexer:\tThe file '{filePath}' does not exist.");
                Environment.Exit(0);
            }
        }

        public void Build()
        {
            EvaluateComments()
            .RemoveComments()
            .Evaluate()
            ;

            foreach (Token token in Tokens)
            {
                Writer.WriteLine(token.Value);
            }

            Environment.Exit(0);
        }
        private Lexer EvaluateComments()
        {
            for (int charIndex = 0; charIndex < Content.Length; charIndex++)
            {
                charIndex = EvaluateComments(charIndex, Content[charIndex]);
            }

            return this;
        }

        private int EvaluateComments(int index, char c)
        {
            switch (c)
            {
                case '-':
                    index = Evaluate(TokenType.Hyphen, index);
                    break;
                case '{':
                    index = Evaluate(TokenType.BraceL, index);
                    break;

                default:
                    break;
            }

            return index;
        }

        private Lexer RemoveComments()
        {
            Tokens.Reverse();

            foreach (Token token in Tokens)
            {
                Content = Content.Remove(token.StartIndex, token.EndIndex - token.StartIndex);
            }

            return this;
        }

        private Lexer Evaluate()
        {
            for (int charIndex = 0; charIndex < Content.Length; charIndex++)
            {
                charIndex = Evaluate(charIndex, Content[charIndex]);
            }

            return this;
        }
        private int Evaluate(int index, char c)
        {
            switch (c)
            {
                default:
                    index = Evaluate(TokenType.Char, index);
                    break;
            }

            return index;
        }
        private int Evaluate(TokenType type, int index)
        {
            Evaluator result;

            switch (type)
            {
                case TokenType.Char:
                    char c = Content[index];
                    switch (c)
                    {
                        case 'm':
                            if (!ModuleStatementFound)
                            {
                                result =
                                    new Evaluator(index, false, Content)
                                        .ShouldBeModuleStatement();

                                if (result.Token != null)
                                {
                                    index = result.Index;
                                    Tokens.Add(result.Token);
                                    ModuleStatementFound = true;
                                }
                            }
                            break;
                        case 'i':
                            if (!AllImportsFound)
                            {

                                result =
                                    new Evaluator(index, false, Content)
                                        .MaybeImportStatement();
                            }
                            else
                            {
                                result =
                                    new Evaluator(index, false, Content);
                            }

                            if (result.Token != null)
                            {
                                index = result.Index;
                                AllImportsFound = result.AllImportsFound;
                                Tokens.Add(result.Token);
                            }
                            break;

                    }

                    break;
                case TokenType.Hyphen:
                    result =
                        new Evaluator(index, false, Content)
                            .MaybeInlineComment();

                    if (result.Token != null)
                    {
                        index = result.Index;
                        Tokens.Add(result.Token);
                    }

                    break;
                case TokenType.BraceL:
                    result =
                        new Evaluator(index, false, Content)
                            .MaybeMultilineComment();

                    if (result.Token != null)
                    {
                        index = result.Index;
                        Tokens.Add(result.Token);
                    }

                    break;
                default:
                    break;
            }

            return index;
        }

        private List<Token> ExtractComments(string value)
        {
            List<Token> commentTokens = new();

            for (int i = 0; i < value.Length; i++)
            {
                if (value[i] == '{')
                {
                    Evaluator r =
                        new Evaluator(i, false, value)
                            .MaybeMultilineComment();

                    if (r.Token != null)
                    {
                        Writer.WriteLine(r.Token.Value);
                        commentTokens.Add(r.Token);
                    }
                }
            }

            commentTokens.Reverse();

            return commentTokens;
        }
    }

    public class Evaluator
    {
        private bool Found { get; set; } = false;

        public bool AllImportsFound { get; set; } = false;
        public int Index { get; set; } = 0;
        public string Content { get; set; } = "";

        public Token? Token;

        public Evaluator(int index, bool found, string content)
        {
            Index = index;
            Found = found;
            Content = content;
        }

        // Module Statement

        public Evaluator ShouldBeModuleStatement()
        {
            if (Found)
            {
                return this;
            }

            string maybeWord = Content[Index..(Index + 7)];

            if (maybeWord == "module " || maybeWord == "module\n")
            {
                int endIndex = FindEndOfModuleStatement(Index + 6);
                string value = Content[Index..endIndex];

                Token = new(value, TokenType.ModuleStatement, Index, endIndex);

                Found = true;
                Index = endIndex;
            }

            return this;

        }

        private int FindEndOfModuleStatement(int index)
        {
            int endIndex = index;

            for (int i = index; i < Content.Length; i++)
            {
                if (Content[i] == '{')
                {
                    i = FindEndOfMultilineComment((0, 0), i);
                }
                else if (Content[i] == ')')
                {
                    endIndex = i + 1;
                    break;
                }
            }

            return endIndex;
        }

        // Import Statements

        public Evaluator MaybeImportStatement()
        {
            if (Found)
            {
                return this;
            }

            string maybeWord = Content[Index..(Index + 7)];

            if (maybeWord == "import " || maybeWord == "import\n")
            {
                (int endIndex, bool isLastImport) = FindEndOfImportStatement(Index + 6);
                string value = Content[Index..endIndex];

                Token = new(value, TokenType.ImportStatement, Index, endIndex);

                AllImportsFound = isLastImport;
                Found = true;
                Index = endIndex;
            }

            return this;
        }

        private (int, bool) FindEndOfImportStatement(int index)
        {
            int endIndex = index;
            bool isLastImport = false;

            for (int i = index; i < Content.Length; i++)
            {
                char c = Content[i];
                char next = Content[i + 1];

                if (c == '\n' && Char.IsLetter(next))
                {
                    string maybeNextImport = Content[(i + 1)..(i + 8)];
                    if (maybeNextImport == "import " || maybeNextImport == "import\n")
                    {
                        endIndex = i;
                        break;
                    }
                    else
                    {
                        endIndex = i;
                        isLastImport = true;
                        break;
                    }
                }
            }

            return (endIndex, isLastImport);
        }


        // Comments
        public Evaluator MaybeInlineComment()
        {
            if (Found)
            {
                return this;
            }

            if (Content[Index + 1] == '-')
            {
                int endIndex = Content.IndexOf('\n', Index);

                if (endIndex == -1)
                {
                    endIndex = Content.Length;
                }

                string value = Content[Index..endIndex];

                Token = new(value, TokenType.InlineComment, Index, endIndex);

                Found = true;
                Index = endIndex;
            }

            return this;
        }

        public Evaluator MaybeMultilineComment()
        {
            if (Found)
            {
                return this;
            }

            if (Content[Index + 1] == '-')
            {
                int startIndex = Index;
                int endIndex = FindEndOfMultilineComment((0, 0), startIndex);
                string value = Content[Index..endIndex];

                Token = new(value, TokenType.MultilineComment, Index, endIndex);

                Found = true;
                Index = endIndex;
            }

            return this;
        }

        private int FindEndOfMultilineComment((int start, int end) counter, int index)
        {
            if (Content[index] == '{' && Content[index + 1] == '-')
            {
                counter.start++;
            }
            else if (Content[index] == '-' && Content[index + 1] == '}')
            {
                counter.end++;
            }

            if (counter.start == counter.end)
            {
                return index + 2;
            }

            return FindEndOfMultilineComment(counter, index + 1);
        }
    }

    public class Token
    {
        public string Value { get; set; } = "";
        public TokenType Type { get; set; } = TokenType.None;

        public int StartIndex { get; set; } = 0;
        public int EndIndex { get; set; } = 0;

        public Token(string value, TokenType type, int startIndex, int endIndex)
        {
            Value = value;
            Type = type;
            StartIndex = startIndex;
            EndIndex = endIndex;
        }
    }

    public enum TokenType
    {
        None,

        BraceL,
        BraceR,
        Char,

        Function,
        Hyphen,

        ImportStatement,
        InlineComment,
        ModuleStatement,
        MultilineComment,
        NewLine,

        Space,

    }

}