namespace Interpreter
{
    public class Lexer
    {
        private SourceReader Source;
        private System.Text.StringBuilder tokenContent = new System.Text.StringBuilder();

        public Lexer(SourceReader source)
        {
            Source = source;
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
            tokenContent.Clear();
            Source.ReadNext();
            if (Source.AtEOF)
                return null;

            while (!Source.AtEOF && char.IsWhiteSpace(Source.CurrentChar))
                Source.ReadNext();

            if (char.IsNumber(Source.CurrentChar))
            {
                tokenContent.Append(Source.CurrentChar);
                while (char.IsNumber(Source.ReadNext()) && !Source.AtEOF)
                    tokenContent.Append(Source.CurrentChar);

                return new Token(Token.Types.Number, tokenContent.ToString());
            }
            else
            {
                tokenContent.Append(Source.CurrentChar);
                while (char.IsLetter(Source.ReadNext()) && !Source.AtEOF)
                    tokenContent.Append(Source.CurrentChar);

                return new Token(Token.Types.Identifier, tokenContent.ToString());
            }
        }
    }

    public class SourceReader
    {
        private System.IO.TextReader sourceStream;
        private System.Collections.Stack charBuffer = new System.Collections.Stack();

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

        public int Peek()
        {
            int nextInStream = sourceStream.Read();
            charBuffer.Push(nextInStream);
            return nextInStream;
        }
    }

    public class Token
    {
        public enum Types { Identifier, Number };

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
            return type.ToString() + ": " + content;
        }
    }
}
