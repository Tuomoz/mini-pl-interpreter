using System;
using System.Collections.Generic;
using System.Linq;

namespace Interpreter
{
    public class Lexer
    {
        private SourceReader Source;
        private System.Text.StringBuilder TokenContentBuilder = new System.Text.StringBuilder();
        private Dictionary<string, Token.Types> OperatorTokens = new Dictionary<string, Token.Types>();
        private Dictionary<char, char> EscapeCharacters = new Dictionary<char, char>();
        private Dictionary<string, Token.Types> KeywordTokens = new Dictionary<string, Token.Types>();

        public Lexer(SourceReader source)
        {
            Source = source;
            OperatorTokens.Add("(", Token.Types.LParen);
            OperatorTokens.Add(")", Token.Types.RParen);
            OperatorTokens.Add("+", Token.Types.OpPlus);
            OperatorTokens.Add("-", Token.Types.OpMinus);
            OperatorTokens.Add("/", Token.Types.OpDivide);
            OperatorTokens.Add("*", Token.Types.OpMultiply);
            OperatorTokens.Add("<", Token.Types.OpLess);
            OperatorTokens.Add("=", Token.Types.OpEquals);
            OperatorTokens.Add("&", Token.Types.OpAnd);
            OperatorTokens.Add("!", Token.Types.OpNot);
            OperatorTokens.Add(";", Token.Types.LineTerm);
            OperatorTokens.Add(":=", Token.Types.OpAssignment);
            OperatorTokens.Add("..", Token.Types.OpRange);
            KeywordTokens.Add("var", Token.Types.KwVar);
            KeywordTokens.Add("int", Token.Types.KwInt);
            KeywordTokens.Add("string", Token.Types.KwString);
            KeywordTokens.Add("bool", Token.Types.KwBool);
            KeywordTokens.Add("for", Token.Types.KwFor);
            KeywordTokens.Add("end", Token.Types.KwEnd);
            KeywordTokens.Add("in", Token.Types.KwIn);
            KeywordTokens.Add("do", Token.Types.KwDo);
            KeywordTokens.Add("read", Token.Types.KwRead);
            KeywordTokens.Add("print", Token.Types.KwPrint);
            KeywordTokens.Add("assert", Token.Types.KwAssert);
            EscapeCharacters.Add('"', '"');
            EscapeCharacters.Add('\'', '\'');
            EscapeCharacters.Add('n', '\n');
            EscapeCharacters.Add('t', '\t');
            EscapeCharacters.Add('\\', '\\');
        }

        public IEnumerable<Token> GetTokens()
        {
            Token nextToken = getNextToken();
            while(nextToken != null)
            {
                yield return nextToken;
                nextToken = getNextToken();
            }
        }

        public Token getNextToken()
        {
            Source.ReadNext();
            SkipWhitespace();
            SkipComments();
            SkipWhitespace();
            if (!Source.CurrentChar.HasValue)
                return null;

            TokenContentBuilder.Clear();
            int newTokenLine = Source.CurrentLine, newTokenColumn = Source.CurrentColumn;

            if (OperatorTokens.ContainsKey(Source.CurrentAndPeek))
            {
                return new Token(OperatorTokens[Source.CurrentAndPeek], newTokenLine, newTokenColumn);
            }
            else if (OperatorTokens.ContainsKey(Source.CurrentChar.ToString()))
            {
                return new Token(OperatorTokens[Source.CurrentChar.ToString()], newTokenLine, newTokenColumn);
            }
            else if (char.IsNumber(Source.CurrentChar.Value))
            {
                TokenContentBuilder.Append(Source.CurrentChar.Value);
                while (Source.Peek().HasValue && char.IsNumber(Source.Peek().Value))
                {
                    TokenContentBuilder.Append(Source.ReadNext());
                }
                return new Token(Token.Types.Number, newTokenLine, newTokenColumn, TokenContentBuilder.ToString());
            }
            else if (char.IsLetter(Source.CurrentChar.Value))
            {
                TokenContentBuilder.Append(Source.CurrentChar.Value);
                while (Source.Peek().HasValue && (char.IsLetterOrDigit(Source.Peek().Value) || Source.Peek() == '_'))
                {
                    TokenContentBuilder.Append(Source.ReadNext());
                }
                string stringToken = TokenContentBuilder.ToString();

                if (KeywordTokens.ContainsKey(stringToken))
                {
                    return new Token(KeywordTokens[stringToken], newTokenLine, newTokenColumn);
                }
                else
                {
                    return new Token(Token.Types.Identifier, newTokenLine, newTokenColumn, stringToken);
                }
            }
            else if (Source.CurrentChar == '"')
            {
                while (Source.Peek() != '"' && Source.Peek() != '\n')
                {
                    if (Source.ReadNext() == '\\')
                    {
                        Source.ReadNext();
                        if (Source.CurrentChar.HasValue && EscapeCharacters.ContainsKey(Source.CurrentChar.Value))
                        {
                            TokenContentBuilder.Append(EscapeCharacters[Source.CurrentChar.Value]);
                        }
                        else
                        {
                            throw new LexerException(
                                string.Format("Unrecognized escape sequence at line {0} column {1}",
                                Source.CurrentLine, Source.CurrentColumn));
                        }
                    }
                    else
                    {
                        TokenContentBuilder.Append(Source.CurrentChar.Value);
                    }
                }
                Source.ReadNext();
                if (!Source.CurrentChar.HasValue || Source.CurrentChar != '"')
                {
                    throw new LexerException(
                        string.Format("EOL while scanning string literal at line {0} column {1}",
                        Source.CurrentLine, Source.CurrentColumn));
                }
                string stringToken = TokenContentBuilder.ToString();
                return new Token(Token.Types.String, newTokenLine, newTokenColumn, stringToken);
            }
            else
            {
                throw new LexerException(
                    string.Format("Unknown token at line {0} column {1}",
                    Source.CurrentLine, Source.CurrentColumn));
            }
        }

        private void SkipWhitespace()
        {
            while (Source.CurrentChar.HasValue && char.IsWhiteSpace(Source.CurrentChar.Value))
                Source.ReadNext();
        }

        private void SkipComments()
        {
            while (Source.CurrentAndPeek == "//" || Source.CurrentAndPeek == "/*")
            {
                if (Source.Peek() == '/')
                {
                    while (Source.CurrentChar.HasValue && Source.CurrentChar != '\n')
                    {
                        Source.ReadNext();
                    }
                }
                else
                {
                    int commentDepth = 1;
                    int commentBeginLine = Source.CurrentLine, commentBeginColumn = Source.CurrentColumn;
                    while (Source.CurrentChar.HasValue && commentDepth > 0)
                    {
                        Source.ReadNext();
                        if (Source.CurrentChar == '/' && Source.Peek() == '*')
                        {
                            commentDepth++;
                            Source.ReadNext();
                        }
                        else if (Source.CurrentChar == '*' && Source.Peek() == '/')
                        {
                            commentDepth--;
                            Source.ReadNext();
                        }
                    }
                    Source.ReadNext();
                    if (commentDepth > 0)
                    {
                        throw new LexerException(
                            string.Format("EOF while scanning comment beginning at line {0} column {1}",
                            commentBeginLine, commentBeginColumn));
                    }
                }
                SkipWhitespace();
            }
        }
    }

    public class SourceReader
    {
        private struct BufferedChar
        {
            public char storedChar;
            public int storedCharColumn, storedCharLine;

            public BufferedChar(char storedChar, int storedCharColumn, int storedCharLine)
            {
                this.storedChar = storedChar;
                this.storedCharColumn = storedCharColumn;
                this.storedCharLine = storedCharLine;
            }
        }

        private System.IO.TextReader sourceStream;
        private int ReaderColumn = 0, ReaderLine = 1;
        private Queue<BufferedChar> charBuffer = new Queue<BufferedChar>();

        public char? CurrentChar { get; private set; }
        public int CurrentColumn { get; private set; } = 0;
        public int CurrentLine { get; private set; } = 0;

        public string CurrentAndPeek
        {
            get { return string.Concat(CurrentChar, Peek()); }
        }

        public SourceReader(System.IO.TextReader sourceStream)
        {
            this.sourceStream = sourceStream;
        }

        public char? ReadNext()
        {
            if (charBuffer.Count > 0)
            {
                BufferedChar buffered = charBuffer.Dequeue();
                CurrentChar = buffered.storedChar;
                CurrentColumn = buffered.storedCharColumn;
                CurrentLine = buffered.storedCharLine;
            }
            else
            {
                CurrentChar = ReadNextFromSource();
                CurrentColumn = ReaderColumn;
                CurrentLine = ReaderLine;
            }
            return CurrentChar;
        }

        public char? Peek(int offset = 0)
        {
            if (charBuffer.Count > offset)
            {
                return charBuffer.ElementAt(offset).storedChar;
            }

            offset -= charBuffer.Count;
            char? nextChar = null;
            for (int i = 0; i <= offset; i++)
            {
                nextChar = ReadNextFromSource();
                if (nextChar.HasValue)
                {
                    charBuffer.Enqueue(new BufferedChar(nextChar.Value, ReaderColumn, ReaderLine));
                }
                else
                {
                    break;
                }
            }
            return nextChar;
        }

        private char? ReadNextFromSource()
        {
            if (CurrentChar.HasValue && CurrentChar.Value == '\n')
            {
                ReaderColumn = 0;
                ReaderLine++;
            }
            int nextChar = sourceStream.Read();
            if (nextChar == '\r')
            {
                nextChar = sourceStream.Read();
            }
            if (nextChar != -1)
            {
                ReaderColumn++;
                return (char)nextChar;
            }
            return null;
        }
    }

    public class Token
    {
        public enum Types { Identifier, Number, LParen, RParen, OpPlus, OpMinus, KwVar, KwInt,
            KwString, OpAssignment, OpRange, String, KwFor, KwEnd, KwIn, KwDo, KwRead, KwPrint,
            KwBool, KwAssert, OpMultiply, OpDivide, OpLess, OpEquals, OpAnd, OpNot, LineTerm };

        public Types Type { get; }
        public string Content { get; }
        public int Column { get; }
        public int Line { get; }

        public Token(Types type, int line, int column)
        {
            Type = type;
            Column = column;
            Line = line;
        }

        public Token(Types type, int line, int column, string content)
        {
            Type = type;
            Column = column;
            Line = line;
            Content = content;
        }

        public override string ToString()
        {
            if (Content != null)
                return string.Format("{0}<{1},{2}>: {3}", Type, Line, Column, Content);
            return string.Format("{0}<{1},{2}>", Type, Line, Column);
        }
    }

    public class LexerException : System.Exception
    {
        public LexerException()
        {
        }

        public LexerException(string message)
            : base(message)
        {
        }

        public LexerException(string message, System.Exception inner)
            : base(message, inner)
        {
        }
    }
}
