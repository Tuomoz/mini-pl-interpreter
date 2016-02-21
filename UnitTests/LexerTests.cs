using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Interpreter;

namespace UnitTests
{
    [TestClass]
    public class LexerTests
    {
        /*
        Basic tests for all token types
        */

        [TestMethod]
        public void TestIntKwToken()
        {
            Lexer lexer = CreateStringLexer("int");
            Token token = lexer.getNextToken();
            Assert.AreEqual(Token.Types.KwInt, token.Type);
        }

        [TestMethod]
        public void TestStringKwToken()
        {
            Lexer lexer = CreateStringLexer("string");
            Token token = lexer.getNextToken();
            Assert.AreEqual(Token.Types.KwString, token.Type);
        }

        [TestMethod]
        public void TestVarKwToken()
        {
            Lexer lexer = CreateStringLexer("var");
            Token token = lexer.getNextToken();
            Assert.AreEqual(Token.Types.KwVar, token.Type);
        }

        [TestMethod]
        public void TestIdentifierToken()
        {
            Lexer lexer = CreateStringLexer("Some1_var");
            Token token = lexer.getNextToken();
            Assert.AreEqual(Token.Types.Identifier, token.Type);
            Assert.AreEqual("some_var", token.Content);
        }

        [TestMethod]
        public void TestLParenToken()
        {
            Lexer lexer = CreateStringLexer("(");
            Token token = lexer.getNextToken();
            Assert.AreEqual(Token.Types.LParen, token.Type);
        }

        [TestMethod]
        public void TestRParenToken()
        {
            Lexer lexer = CreateStringLexer(")");
            Token token = lexer.getNextToken();
            Assert.AreEqual(Token.Types.RParen, token.Type);
        }

        [TestMethod]
        public void TestNumberToken()
        {
            Lexer lexer = CreateStringLexer("123");
            Token token = lexer.getNextToken();
            Assert.AreEqual(Token.Types.Number, token.Type);
        }

        [TestMethod]
        public void TestOpAssignmentToken()
        {
            Lexer lexer = CreateStringLexer(":=");
            Token token = lexer.getNextToken();
            Assert.AreEqual(Token.Types.OpAssignment, token.Type);
        }

        [TestMethod]
        public void TestOpMinusToken()
        {
            Lexer lexer = CreateStringLexer("-");
            Token token = lexer.getNextToken();
            Assert.AreEqual(Token.Types.OpMinus, token.Type);
        }

        [TestMethod]
        public void TestOpPlusToken()
        {
            Lexer lexer = CreateStringLexer("+");
            Token token = lexer.getNextToken();
            Assert.AreEqual(Token.Types.OpPlus, token.Type);
        }

        [TestMethod]
        public void TestOpRangeToken()
        {
            Lexer lexer = CreateStringLexer("..");
            Token token = lexer.getNextToken();
            Assert.AreEqual(Token.Types.OpRange, token.Type);
        }

        [TestMethod]
        public void TestStringToken()
        {
            Lexer lexer = CreateStringLexer("\"some string\"");
            Token token = lexer.getNextToken();
            Assert.AreEqual(Token.Types.String, token.Type);
            Assert.AreEqual("some string", token.Content);
        }

        /*
        Advanced tests
        */

        [TestMethod]
        public void TestMultipleTokensOnSameLine()
        {
            Lexer lexer = CreateStringLexer("int string var");
            Token token = lexer.getNextToken();
            Assert.AreEqual(Token.Types.KwInt, token.Type);
            Assert.AreEqual(1, token.Column);
            Assert.AreEqual(1, token.Line);
            token = lexer.getNextToken();
            Assert.AreEqual(Token.Types.KwString, token.Type);
            Assert.AreEqual(5, token.Column);
            Assert.AreEqual(1, token.Line);
            token = lexer.getNextToken();
            Assert.AreEqual(Token.Types.KwVar, token.Type);
            Assert.AreEqual(12, token.Column);
            Assert.AreEqual(1, token.Line);
        }

        [TestMethod]
        public void TestTokensOnMultipleLines()
        {
            Lexer lexer = CreateStringLexer("int\nstring var");
            Token token = lexer.getNextToken();
            Assert.AreEqual(Token.Types.KwInt, token.Type);
            Assert.AreEqual(1, token.Column);
            Assert.AreEqual(1, token.Line);
            token = lexer.getNextToken();
            Assert.AreEqual(Token.Types.KwString, token.Type);
            Assert.AreEqual(1, token.Column);
            Assert.AreEqual(2, token.Line);
            token = lexer.getNextToken();
            Assert.AreEqual(Token.Types.KwVar, token.Type);
            Assert.AreEqual(8, token.Column);
            Assert.AreEqual(2, token.Line);
        }

        [TestMethod]
        public void TestSkippingWhitespace()
        {
            Lexer lexer = CreateStringLexer("int \n\tvar");
            Token token = lexer.getNextToken();
            Assert.AreEqual(Token.Types.KwInt, token.Type);
            Assert.AreEqual(1, token.Column);
            Assert.AreEqual(1, token.Line);
            token = lexer.getNextToken();
            Assert.AreEqual(Token.Types.KwVar, token.Type);
            Assert.AreEqual(2, token.Column);
            Assert.AreEqual(2, token.Line);
        }

        [TestMethod]
        public void TestStringEscapes()
        {
            Lexer lexer = CreateStringLexer("\"asd \\\"\\n\\t\\\\\"");
            Assert.AreEqual("asd \"\n\t\\", lexer.getNextToken().Content);
        }

        [TestMethod]
        public void TestUnknownStringEscape()
        {
            Lexer lexer = CreateStringLexer("\"asd \\a\"");
            try
            {
                lexer.getNextToken();
            }
            catch (LexerException e)
            {
                StringAssert.Equals(e, "Unrecognized escape sequence at line 1 column 7");
                return;
            }
            Assert.Fail("No exception was thrown.");
        }

        [TestMethod]
        public void TestUnclosedString()
        {
            Lexer lexer = CreateStringLexer("\"asd\n");
            try
            {
                lexer.getNextToken();
            }
            catch (LexerException e)
            {
                StringAssert.Equals(e, "EOL while scanning string literal at line 1 column 5");
                return;
            }
            Assert.Fail("No exception was thrown.");
        }

        [TestMethod]
        public void TestUnknownToken()
        {
            Lexer lexer = CreateStringLexer("@");
            try
            {
                lexer.getNextToken();
            }
            catch (LexerException e)
            {
                StringAssert.Equals(e, "Unknown token at line 1 column 1");
                return;
            }
            Assert.Fail("No exception was thrown.");
        }

        /*
        Tests for comments
        */

        [TestMethod]
        public void Scanner_WithSingleLineComment_SkipsCommentedCode()
        {
            Lexer lexer = CreateStringLexer("var//int\nstring");
            Assert.AreEqual(Token.Types.KwVar, lexer.getNextToken().Type);
            Assert.AreEqual(Token.Types.KwString, lexer.getNextToken().Type);
        }

        [TestMethod]
        public void Scanner_WithSingleLineCommentAndWhitespace_SkipsCommentedCode()
        {
            Lexer lexer = CreateStringLexer("var //int \n \n  string");
            Assert.AreEqual(Token.Types.KwVar, lexer.getNextToken().Type);
            Assert.AreEqual(Token.Types.KwString, lexer.getNextToken().Type);
        }

        [TestMethod]
        public void Scanner_WithSingleLineCommentAtBeginningOfLine_SkipsCommentedCode()
        {
            Lexer lexer = CreateStringLexer("//int\nstring");
            Assert.AreEqual(Token.Types.KwString, lexer.getNextToken().Type);
        }

        [TestMethod]
        public void Scanner_WithMultipleSingleLineComments_SkipsCommentedCode()
        {
            Lexer lexer = CreateStringLexer("var //int//123\n  //456\nstring");
            Assert.AreEqual(Token.Types.KwVar, lexer.getNextToken().Type);
            Assert.AreEqual(Token.Types.KwString, lexer.getNextToken().Type);
        }

        [TestMethod]
        public void Scanner_WithMultilineCommentOnOneLine_SkipsCommentedCode()
        {
            Lexer lexer = CreateStringLexer("var/*int*/\nstring");
            Assert.AreEqual(Token.Types.KwVar, lexer.getNextToken().Type);
            Assert.AreEqual(Token.Types.KwString, lexer.getNextToken().Type);
        }

        [TestMethod]
        public void Scanner_WithMultilineCommentOnMultipleLines_SkipsCommentedCode()
        {
            Lexer lexer = CreateStringLexer("var/*int\n123\nabc*/\nstring");
            Assert.AreEqual(Token.Types.KwVar, lexer.getNextToken().Type);
            Assert.AreEqual(Token.Types.KwString, lexer.getNextToken().Type);
        }

        [TestMethod]
        public void Scanner_WithNestedMultilineComments_SkipsCommentedCode()
        {
            Lexer lexer = CreateStringLexer("var/*int/*123*/abc*/\nstring");
            Assert.AreEqual(Token.Types.KwVar, lexer.getNextToken().Type);
            Assert.AreEqual(Token.Types.KwString, lexer.getNextToken().Type);
        }

        [TestMethod]
        public void Scanner_WithMultilineCommentAndWhitespace_SkipsCommentedCode()
        {
            Lexer lexer = CreateStringLexer("var \n/*int*/ \n \n  string");
            Assert.AreEqual(Token.Types.KwVar, lexer.getNextToken().Type);
            Assert.AreEqual(Token.Types.KwString, lexer.getNextToken().Type);
        }

        [TestMethod]
        public void Scanner_WithMultilineCommentEndingAtFileEnd_SkipsCommentedCode()
        {
            Lexer lexer = CreateStringLexer("var/*int\nstring*/");
            Assert.AreEqual(Token.Types.KwVar, lexer.getNextToken().Type);
            Assert.IsNull(lexer.getNextToken());
        }

        [TestMethod]
        public void Scanner_WithMultilineCommentAtBeginningOfLine_SkipsCommentedCode()
        {
            Lexer lexer = CreateStringLexer("/*int*/string");
            Assert.AreEqual(Token.Types.KwString, lexer.getNextToken().Type);
        }

        [TestMethod]
        public void Scanner_WithMultilineCommentBetweenCodeOnSameLine_SkipsCommentedCode()
        {
            Lexer lexer = CreateStringLexer("var /* int */ string");
            Assert.AreEqual(Token.Types.KwVar, lexer.getNextToken().Type);
            Assert.AreEqual(Token.Types.KwString, lexer.getNextToken().Type);
        }

        [TestMethod]
        public void Scanner_WithUnclosedMultilineComment_ThrowsException()
        {
            Lexer lexer = CreateStringLexer("var/*int\nstring");
            Assert.AreEqual(Token.Types.KwVar, lexer.getNextToken().Type);
            try
            {
                lexer.getNextToken();
            }
            catch (LexerException e)
            {
                StringAssert.Equals(e, "EOF while scanning comment beginning at line 1 column 4");
                return;
            }
            Assert.Fail("No exception was thrown.");
        }

        [TestMethod]
        public void Scanner_WithSingleAndMultilineComments_SkipsCommentedCode()
        {
            Lexer lexer = CreateStringLexer("var /* int */ //123\n/*321*/string");
            Assert.AreEqual(Token.Types.KwVar, lexer.getNextToken().Type);
            Assert.AreEqual(Token.Types.KwString, lexer.getNextToken().Type);
        }

        [TestMethod]
        public void Scanner_WithSingleLineCommentInsideMultiline_SkipsCommentedCode()
        {
            Lexer lexer = CreateStringLexer("var /* //int */string");
            Assert.AreEqual(Token.Types.KwVar, lexer.getNextToken().Type);
            Assert.AreEqual(Token.Types.KwString, lexer.getNextToken().Type);
        }

        [TestMethod]
        public void Scanner_WithMultilineCommentInsideSingleLine_SkipsCommentedCode()
        {
            Lexer lexer = CreateStringLexer("var ///*123\nstring");
            Assert.AreEqual(Token.Types.KwVar, lexer.getNextToken().Type);
            Assert.AreEqual(Token.Types.KwString, lexer.getNextToken().Type);
        }

        public Lexer CreateStringLexer(string source)
        {
            SourceReader reader = new SourceReader(new System.IO.StringReader(source));
            return new Lexer(reader);
        }
    }
}
