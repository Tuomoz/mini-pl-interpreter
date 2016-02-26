using System.Collections.Generic;
using System.Linq;

namespace Lexer
{
    public class Scanner
    {
        private SourceReader Source;
        private Dictionary<string, Token.Types> SymbolTokens = new Dictionary<string, Token.Types>();
        private Dictionary<char, char> EscapeCharacters = new Dictionary<char, char>();
        private Dictionary<string, Token.Types> KeywordTokens = new Dictionary<string, Token.Types>();

        public Scanner(SourceReader source)
        {
            Source = source;
            SymbolTokens.Add("(", Token.Types.LParen);
            SymbolTokens.Add(")", Token.Types.RParen);
            SymbolTokens.Add("+", Token.Types.OpPlus);
            SymbolTokens.Add("-", Token.Types.OpMinus);
            SymbolTokens.Add("/", Token.Types.OpDivide);
            SymbolTokens.Add("*", Token.Types.OpMultiply);
            SymbolTokens.Add("<", Token.Types.OpLess);
            SymbolTokens.Add("=", Token.Types.OpEquals);
            SymbolTokens.Add("&", Token.Types.OpAnd);
            SymbolTokens.Add("!", Token.Types.OpNot);
            SymbolTokens.Add(";", Token.Types.LineTerm);
            SymbolTokens.Add(":", Token.Types.Colon);
            SymbolTokens.Add(":=", Token.Types.OpAssignment);
            SymbolTokens.Add("..", Token.Types.OpRange);
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
            Token nextToken = GetNextToken();
            while(nextToken != null)
            {
                yield return nextToken;
                nextToken = GetNextToken();
            }
        }

        public Token GetNextToken()
        {
            Source.ReadNext();
            SkipWhitespace();
            SkipComments();
            SkipWhitespace();
            if (!Source.CurrentChar.HasValue)
                return null;

            int newTokenLine = Source.CurrentLine, newTokenColumn = Source.CurrentColumn;

            if (SymbolTokens.ContainsKey(Source.CurrentAndPeek))
            {
                return new Token(SymbolTokens[Source.CurrentAndPeek], newTokenLine, newTokenColumn);
            }
            else if (SymbolTokens.ContainsKey(Source.CurrentChar.ToString()))
            {
                return new Token(SymbolTokens[Source.CurrentChar.ToString()], newTokenLine, newTokenColumn);
            }
            else if (char.IsNumber(Source.CurrentChar.Value))
            {
                string tokenContent = Source.ReadWhile(peeked => char.IsDigit(peeked));
                return new Token(Token.Types.IntLiteral, newTokenLine, newTokenColumn, tokenContent);
            }
            else if (char.IsLetter(Source.CurrentChar.Value))
            {
                string tokenContent = Source.ReadWhile(peeked => char.IsLetterOrDigit(peeked) || peeked == '_');
                if (KeywordTokens.ContainsKey(tokenContent))
                {
                    return new Token(KeywordTokens[tokenContent], newTokenLine, newTokenColumn);
                }
                else
                {
                    return new Token(Token.Types.Identifier, newTokenLine, newTokenColumn, tokenContent);
                }
            }
            else if (Source.CurrentChar == '"')
            {
                string stringToken = BuildStringLiteral();
                return new Token(Token.Types.StringLiteral, newTokenLine, newTokenColumn, stringToken);
            }
            else
            {
                throw new LexerException(
                    string.Format("Unknown token '{0}' at line {1} column {2}",
                    Source.CurrentChar, Source.CurrentLine, Source.CurrentColumn));
            }
        }

        private string BuildStringLiteral()
        {
            System.Text.StringBuilder TokenContentBuilder = new System.Text.StringBuilder();
            while (Source.Peek() != '"' && Source.Peek() != '\n')
            {
                if (Source.ReadNext() == '\\')
                {
                    Source.ReadNext();
                    try
                    {
                        TokenContentBuilder.Append(EscapeCharacters[Source.CurrentChar.Value]);
                    }
                    catch (KeyNotFoundException)
                    {
                        throw new LexerException(
                            string.Format("Unrecognized escape sequence '\\{0}' at line {1} column {2}",
                            Source.CurrentChar, Source.CurrentLine, Source.CurrentColumn));
                    }
                }
                else
                {
                    TokenContentBuilder.Append(Source.CurrentChar.Value);
                }
            }
            Source.ReadNext();
            if (Source.CurrentChar != '"')
            {
                throw new LexerException(
                    string.Format("EOL while scanning string literal at line {0} column {1}",
                    Source.CurrentLine, Source.CurrentColumn));
            }
            return TokenContentBuilder.ToString();
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
                    while (Source.CurrentChar != '\n')
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
            public char StoredChar;
            public int StoredCharColumn, StoredCharLine;

            public BufferedChar(char storedChar, int storedCharColumn, int storedCharLine)
            {
                StoredChar = storedChar;
                StoredCharColumn = storedCharColumn;
                StoredCharLine = storedCharLine;
            }
        }

        private System.IO.TextReader SourceStream;
        private int ReaderColumn = 0, ReaderLine = 1;
        private Queue<BufferedChar> CharBuffer = new Queue<BufferedChar>();

        public char? CurrentChar { get; private set; }
        public int CurrentColumn { get; private set; } = 0;
        public int CurrentLine { get; private set; } = 0;

        public string CurrentAndPeek
        {
            get { return string.Concat(CurrentChar, Peek()); }
        }

        public SourceReader(System.IO.TextReader sourceStream)
        {
            SourceStream = sourceStream;
        }

        public char? ReadNext()
        {
            if (CharBuffer.Count > 0)
            {
                BufferedChar buffered = CharBuffer.Dequeue();
                CurrentChar = buffered.StoredChar;
                CurrentColumn = buffered.StoredCharColumn;
                CurrentLine = buffered.StoredCharLine;
            }
            else
            {
                CurrentChar = ReadNextFromSource();
                CurrentColumn = ReaderColumn;
                CurrentLine = ReaderLine;
            }
            return CurrentChar;
        }

        public string ReadWhile(System.Func<char, bool> testFunc)
        {
            System.Text.StringBuilder content = new System.Text.StringBuilder();
            content.Append(CurrentChar);
            while (Peek().HasValue && testFunc(Peek().Value))
            {
                content.Append(ReadNext());
            }
            return content.ToString();
        }

        public char? Peek(int offset = 0)
        {
            if (CharBuffer.Count > offset)
            {
                return CharBuffer.ElementAt(offset).StoredChar;
            }

            offset -= CharBuffer.Count;
            char? nextChar = null;
            for (int i = 0; i <= offset; i++)
            {
                nextChar = ReadNextFromSource();
                if (nextChar.HasValue)
                {
                    CharBuffer.Enqueue(new BufferedChar(nextChar.Value, ReaderColumn, ReaderLine));
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
            int nextChar = SourceStream.Read();
            if (nextChar == '\r')
            {
                nextChar = SourceStream.Read();
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
        public enum Types { Identifier, IntLiteral, LParen, RParen, OpPlus, OpMinus, KwVar, KwInt,
            KwString, OpAssignment, OpRange, StringLiteral, KwFor, KwEnd, KwIn, KwDo, KwRead, KwPrint,
            KwBool, KwAssert, OpMultiply, OpDivide, OpLess, OpEquals, OpAnd, OpNot, LineTerm, Colon
        };

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
