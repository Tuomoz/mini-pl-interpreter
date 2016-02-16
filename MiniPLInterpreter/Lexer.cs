using System.Collections.Generic;

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
            TokenBuilder.Clear();
            char? peeked;
            Source.ReadNext();
            while (Source.CurrentChar.HasValue && char.IsWhiteSpace(Source.CurrentChar.Value))
                Source.ReadNext();
            if (!Source.CurrentChar.HasValue)
                return null;

            if (SingleCharTokens.ContainsKey(Source.CurrentChar.Value))
            {
                return new Token(SingleCharTokens[Source.CurrentChar.Value]);
            }
            else if (char.IsNumber(Source.CurrentChar.Value))
            {
                TokenBuilder.Append(Source.CurrentChar.Value);
                for (peeked = Source.Peek(); peeked.HasValue && char.IsNumber(peeked.Value); peeked = Source.Peek())
                {
                    TokenBuilder.Append(Source.ReadNext());
                }
                return new Token(Token.Types.Number, TokenBuilder.ToString());
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
                    return new Token(KeywordTokens[stringToken]);
                return new Token(Token.Types.Identifier, stringToken);
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
        private Stack<BufferedChar> charBuffer = new Stack<BufferedChar>();
        private int ReaderColumn = 0, ReaderLineNumber = 0;

        public char? CurrentChar { get; private set; }
        public int CurrentColumn { get; private set; } = 0;
        public int CurrentLineNumber { get; private set; } = 0;

        public SourceReader(System.IO.TextReader sourceStream)
        {
            this.sourceStream = sourceStream;
        }

        public char? ReadNext()
        {
            if (charBuffer.Count > 0)
            {
                BufferedChar buffered = charBuffer.Pop();
                CurrentChar = buffered.storedChar;
                CurrentColumn = buffered.storedCharColumn;
                CurrentLineNumber = buffered.storedCharLine;
            }
            else
            {
                CurrentChar = ReadNextFromSource();
                CurrentColumn = ReaderColumn;
                CurrentLineNumber = ReaderLineNumber;
            }
            return CurrentChar;
        }

        public char? Peek()
        {
            char? nextChar = ReadNextFromSource();
            if (nextChar.HasValue)
            {
                charBuffer.Push(new BufferedChar(nextChar.Value, ReaderColumn, ReaderLineNumber));
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

        Types type;
        string content;

        public Token(Types type)
        {
            this.type = type;
        }

        public Token(Types type, string content)
        {
            this.type = type;
            this.content = content;
        }

        public override string ToString()
        {
            if (content != null)
                return string.Format("{0}: {1}", type, content);
            return type.ToString();
        }
    }
}
