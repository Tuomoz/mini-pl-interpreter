using System;
using System.Collections.Generic;
using System.Linq;
using Lexer;

namespace Interpreter
{
    class Program
    {
        static void Main(string[] args)
        {
            SourceReader source = new SourceReader(new System.IO.StringReader("var test 123"));
            Scanner lexer = new Scanner(source);
            foreach (Token token in lexer.GetTokens())
                System.Console.WriteLine(token);
        }
    }
}
