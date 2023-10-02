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
            for (int charIndex = 0; charIndex < Content.Length; charIndex++)
            {
                char c = Content[charIndex];

                charIndex = Evaluate(charIndex, c);

                if (charIndex == -1)
                {
                    break;
                }

            }
            foreach (Token token in Tokens)
            {
                Writer.WriteLine(token.Value);
            }
            Environment.Exit(0);
        }

        private int Evaluate(int index, char c)
        {
            switch (c)
            {
                case '-':
                    index = Evaluate(TokenType.Hyphen, index);
                    break;
                case '{':
                    index = Evaluate(TokenType.BraceL, index);
                    break;
                case '\n':
                    break;

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
                    if (c == 'm' && !ModuleStatementFound)
                    {
                        result =
                            new Evaluator(index, false, Content)
                                .MaybeModuleStatement();
                    }
                    else
                    {
                        result =
                            new Evaluator(-1, true, Content);
                    }

                    if (result.Token != null)
                    {
                        index = result.Index;
                        Tokens.Add(result.Token);
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
                    Writer.WriteLine($"ElmX.Elm.Lexer:\tThe token type '{type}' is not supported.");

                    index = -1;
                    break;
            }

            return index;
        }
    }

    public class Evaluator
    {
        private bool Found { get; set; } = false;

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

        public Evaluator MaybeModuleStatement()
        {
            if (Found)
            {
                return this;
            }

            string maybeWord = Content[Index..(Index + 6)];

            if (maybeWord == "module")
            {
                int endIndex = FindEndOfModuleStatement(Index + 6);
                Writer.WriteLine($"ElmX.Elm.Evaluator:\tFound module statement at index {Index} to {endIndex}.");
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
        InlineComment,
        ModuleStatement,
        MultilineComment,
        NewLine,

        Space,

    }

}