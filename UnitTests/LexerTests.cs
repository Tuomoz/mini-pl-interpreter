using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Interpreter;

namespace UnitTests
{
    [TestClass]
    public class LexerTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            SourceReader source = new SourceReader(new System.IO.StringReader("123 456 moi"));
            Lexer lexer = new Lexer(source);
            Assert.AreEqual(lexer.getNextToken().ToString(), "Number: 123");
        }
    }
}
