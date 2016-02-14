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

        public System.Collections.Generic.IEnumerable<Token> GetTokens()
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
            Source.ReadNext();
            if (Source.AtEOF)
                return null;

            while (!Source.AtEOF && char.IsWhiteSpace(Source.CurrentChar))
                Source.ReadNext();
            if (SingleCharTokens.ContainsKey(Source.CurrentChar))
            {
                return new Token(SingleCharTokens[Source.CurrentChar]);
            }
            else if (char.IsNumber(Source.CurrentChar))
            {
                TokenBuilder.Append(Source.CurrentChar);
                while (char.IsNumber(Source.Peek()) && !Source.AtEOF)
                    TokenBuilder.Append(Source.ReadNext());

                return new Token(Token.Types.Number, TokenBuilder.ToString());
            }
            else
            {
                TokenBuilder.Append(Source.CurrentChar);
                while (char.IsLetterOrDigit(Source.Peek()) && !Source.AtEOF)
                    TokenBuilder.Append(Source.ReadNext());
                string stringToken = TokenBuilder.ToString();

                if (KeywordTokens.ContainsKey(stringToken))
                    return new Token(KeywordTokens[stringToken]);
                return new Token(Token.Types.Identifier, stringToken);
            }
        }
    }

    public class SourceReader
    {
        private System.IO.TextReader sourceStream;
        private Stack<int> charBuffer = new Stack<int>();

        public char CurrentChar { get; private set; }
        public bool AtEOF { get; private set; }

        public SourceReader(System.IO.TextReader sourceStream)
        {
            this.sourceStream = sourceStream;
            AtEOF = false;
        }

        public char ReadNext()
        {
            if (charBuffer.Count > 0)
                CurrentChar = (char) charBuffer.Pop();
            else
            {
                int nextChar = sourceStream.Read();
                if (nextChar < 0)
                    AtEOF = true;
                CurrentChar = (char)nextChar;
            }
            return CurrentChar;
        }

        public char Peek()
        {
            int nextChar = sourceStream.Read();
            charBuffer.Push(nextChar);
            return (char) nextChar;
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
