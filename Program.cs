using System;

namespace Interpreter
{
    class Program
    {
        static void Main(string[] args)
        {
            SourceReader source = new SourceReader(new System.IO.StringReader("123 456 moi"));
            Lexer lexer = new Lexer(source);
            foreach (Token token in lexer.GetTokens())
                System.Console.WriteLine(token);
        }
    }
}
