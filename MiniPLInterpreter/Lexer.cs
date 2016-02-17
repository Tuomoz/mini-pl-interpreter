using System.Collections.Generic;
using System.Linq;

namespace Interpreter
{
    public class Lexer
    {
        private SourceReader Source;
        private System.Text.StringBuilder TokenBuilder = new System.Text.StringBuilder();
        private Dictionary<char, Token.Types> SingleCharTokens = new Dictionary<char, Token.Types>();
        private Dictionary<string, Token.Types> KeywordTokens = new Dictionary<string, Token.Types>();

        public Lexer(SourceReader source)
        {
            Source = source;
            SingleCharTokens.Add('(', Token.Types.LParen);
            SingleCharTokens.Add(')', Token.Types.RParen);
            SingleCharTokens.Add('+', Token.Types.OpPlus);
            SingleCharTokens.Add('-', Token.Types.OpMinus);
            KeywordTokens.Add("var", Token.Types.KwVar);
            KeywordTokens.Add("int", Token.Types.KwInt);
            KeywordTokens.Add("string", Token.Types.KwString);
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
            while (Source.CurrentChar.HasValue && char.IsWhiteSpace(Source.CurrentChar.Value))
                Source.ReadNext();
            if (!Source.CurrentChar.HasValue)
                return null;

            TokenBuilder.Clear();
            char? peeked;
            int newTokenLine = Source.CurrentLine, newTokenColumn = Source.CurrentColumn;

            if (SingleCharTokens.ContainsKey(Source.CurrentChar.Value))
            {
                return new Token(SingleCharTokens[Source.CurrentChar.Value], newTokenLine, newTokenColumn);
            }
            else if (char.IsNumber(Source.CurrentChar.Value))
            {
                TokenBuilder.Append(Source.CurrentChar.Value);
                for (peeked = Source.Peek(); peeked.HasValue && char.IsNumber(peeked.Value); peeked = Source.Peek())
                {
                    TokenBuilder.Append(Source.ReadNext());
                }
                return new Token(Token.Types.Number, newTokenLine, newTokenColumn, TokenBuilder.ToString());
            }
            else
            {
                TokenBuilder.Append(Source.CurrentChar.Value);
                for (peeked = Source.Peek(); peeked.HasValue && char.IsLetterOrDigit(peeked.Value); peeked = Source.Peek())
                {
                    TokenBuilder.Append(Source.ReadNext());
                }
                string stringToken = TokenBuilder.ToString();

                if (KeywordTokens.ContainsKey(stringToken))
                    return new Token(KeywordTokens[stringToken], newTokenLine, newTokenColumn);
                return new Token(Token.Types.Identifier, newTokenLine, newTokenColumn, stringToken);
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
        private string CurrentReaderLine;
        private int ReaderColumn = 0, ReaderLineNumber = -1;
        private Queue<BufferedChar> charBuffer = new Queue<BufferedChar>();

        public char? CurrentChar { get; private set; }
        public int CurrentColumn { get; private set; } = 0;
        public int CurrentLine { get; private set; } = 0;

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
                CurrentColumn = ReaderColumn + 1;
                CurrentLine = ReaderLineNumber + 1;
            }
            return CurrentChar;
        }

        public char? Peek()
        {
            return Peek(0);
        }

        public char? Peek(int offset)
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
                    charBuffer.Enqueue(new BufferedChar(nextChar.Value, ReaderColumn + 1, ReaderLineNumber + 1));
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
            if (CurrentReaderLine != null && ReaderColumn < CurrentReaderLine.Length - 1)
            {
                ReaderColumn++;
            }
            else
            {
                CurrentReaderLine = sourceStream.ReadLine();
                if (CurrentReaderLine == null)
                {
                    return null;
                }
                ReaderColumn = 0;
                ReaderLineNumber++;
            }
            return CurrentReaderLine[ReaderColumn];
        }
    }

    public class Token
    {
        public enum Types { Identifier, Number, LParen, RParen, OpPlus, OpMinus, KwVar, KwInt, KwString};

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
}
