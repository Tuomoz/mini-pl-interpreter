using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lexer;

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
            Scanner lexer = CreateStringLexer("int");
            Token token = lexer.GetNextToken();
            Assert.AreEqual(Token.Types.KwInt, token.Type);
        }

        [TestMethod]
        public void TestStringKwToken()
        {
            Scanner lexer = CreateStringLexer("string");
            Token token = lexer.GetNextToken();
            Assert.AreEqual(Token.Types.KwString, token.Type);
        }

        [TestMethod]
        public void TestVarKwToken()
        {
            Scanner lexer = CreateStringLexer("var");
            Token token = lexer.GetNextToken();
            Assert.AreEqual(Token.Types.KwVar, token.Type);
        }

        [TestMethod]
        public void TestForKwToken()
        {
            Scanner lexer = CreateStringLexer("for");
            Token token = lexer.GetNextToken();
            Assert.AreEqual(Token.Types.KwFor, token.Type);
        }

        [TestMethod]
        public void TestEndKwToken()
        {
            Scanner lexer = CreateStringLexer("end");
            Token token = lexer.GetNextToken();
            Assert.AreEqual(Token.Types.KwEnd, token.Type);
        }

        [TestMethod]
        public void TestInKwToken()
        {
            Scanner lexer = CreateStringLexer("in");
            Token token = lexer.GetNextToken();
            Assert.AreEqual(Token.Types.KwIn, token.Type);
        }

        [TestMethod]
        public void TestDoKwToken()
        {
            Scanner lexer = CreateStringLexer("do");
            Token token = lexer.GetNextToken();
            Assert.AreEqual(Token.Types.KwDo, token.Type);
        }

        [TestMethod]
        public void TestReadKwToken()
        {
            Scanner lexer = CreateStringLexer("read");
            Token token = lexer.GetNextToken();
            Assert.AreEqual(Token.Types.KwRead, token.Type);
        }

        [TestMethod]
        public void TestPrintKwToken()
        {
            Scanner lexer = CreateStringLexer("print");
            Token token = lexer.GetNextToken();
            Assert.AreEqual(Token.Types.KwPrint, token.Type);
        }

        [TestMethod]
        public void TestBoolKwToken()
        {
            Scanner lexer = CreateStringLexer("bool");
            Token token = lexer.GetNextToken();
            Assert.AreEqual(Token.Types.KwBool, token.Type);
        }

        [TestMethod]
        public void TestAssertKwToken()
        {
            Scanner lexer = CreateStringLexer("assert");
            Token token = lexer.GetNextToken();
            Assert.AreEqual(Token.Types.KwAssert, token.Type);
        }

        [TestMethod]
        public void TestIdentifierToken()
        {
            Scanner lexer = CreateStringLexer("Some1_var");
            Token token = lexer.GetNextToken();
            Assert.AreEqual(Token.Types.Identifier, token.Type);
            Assert.AreEqual("Some1_var", token.Content);
        }

        [TestMethod]
        public void TestLParenToken()
        {
            Scanner lexer = CreateStringLexer("(");
            Token token = lexer.GetNextToken();
            Assert.AreEqual(Token.Types.LParen, token.Type);
        }

        [TestMethod]
        public void TestRParenToken()
        {
            Scanner lexer = CreateStringLexer(")");
            Token token = lexer.GetNextToken();
            Assert.AreEqual(Token.Types.RParen, token.Type);
        }

        [TestMethod]
        public void TestNumberToken()
        {
            Scanner lexer = CreateStringLexer("123");
            Token token = lexer.GetNextToken();
            Assert.AreEqual(Token.Types.IntLiteral, token.Type);
            Assert.AreEqual("123", token.Content);
        }

        [TestMethod]
        public void TestOpAssignmentToken()
        {
            Scanner lexer = CreateStringLexer(":=");
            Token token = lexer.GetNextToken();
            Assert.AreEqual(Token.Types.OpAssignment, token.Type);
        }

        [TestMethod]
        public void TestColonToken()
        {
            Scanner lexer = CreateStringLexer(":");
            Token token = lexer.GetNextToken();
            Assert.AreEqual(Token.Types.Colon, token.Type);
        }

        [TestMethod]
        public void TestOpMinusToken()
        {
            Scanner lexer = CreateStringLexer("-");
            Token token = lexer.GetNextToken();
            Assert.AreEqual(Token.Types.OpMinus, token.Type);
        }

        [TestMethod]
        public void TestOpPlusToken()
        {
            Scanner lexer = CreateStringLexer("+");
            Token token = lexer.GetNextToken();
            Assert.AreEqual(Token.Types.OpPlus, token.Type);
        }

        [TestMethod]
        public void TestOpRangeToken()
        {
            Scanner lexer = CreateStringLexer("..");
            Token token = lexer.GetNextToken();
            Assert.AreEqual(Token.Types.OpRange, token.Type);
        }

        [TestMethod]
        public void TestOpMultiplyToken()
        {
            Scanner lexer = CreateStringLexer("*");
            Token token = lexer.GetNextToken();
            Assert.AreEqual(Token.Types.OpMultiply, token.Type);
        }

        [TestMethod]
        public void TestOpDivideToken()
        {
            Scanner lexer = CreateStringLexer("/");
            Token token = lexer.GetNextToken();
            Assert.AreEqual(Token.Types.OpDivide, token.Type);
        }

        [TestMethod]
        public void TestOpLessToken()
        {
            Scanner lexer = CreateStringLexer("<");
            Token token = lexer.GetNextToken();
            Assert.AreEqual(Token.Types.OpLess, token.Type);
        }

        [TestMethod]
        public void TestOpEqualsToken()
        {
            Scanner lexer = CreateStringLexer("=");
            Token token = lexer.GetNextToken();
            Assert.AreEqual(Token.Types.OpEquals, token.Type);
        }

        [TestMethod]
        public void TestOpAndToken()
        {
            Scanner lexer = CreateStringLexer("&");
            Token token = lexer.GetNextToken();
            Assert.AreEqual(Token.Types.OpAnd, token.Type);
        }

        [TestMethod]
        public void TestTerminatorToken()
        {
            Scanner lexer = CreateStringLexer(";");
            Token token = lexer.GetNextToken();
            Assert.AreEqual(Token.Types.LineTerm, token.Type);
        }

        [TestMethod]
        public void TestStringToken()
        {
            Scanner lexer = CreateStringLexer("\"some string\"");
            Token token = lexer.GetNextToken();
            Assert.AreEqual(Token.Types.StringLiteral, token.Type);
            Assert.AreEqual("some string", token.Content);
        }

        /*
        Advanced tests
        */

        [TestMethod]
        public void TestMultipleTokensOnSameLine()
        {
            Scanner lexer = CreateStringLexer("int string var");
            Token token = lexer.GetNextToken();
            Assert.AreEqual(Token.Types.KwInt, token.Type);
            Assert.AreEqual(1, token.Column);
            Assert.AreEqual(1, token.Line);
            token = lexer.GetNextToken();
            Assert.AreEqual(Token.Types.KwString, token.Type);
            Assert.AreEqual(5, token.Column);
            Assert.AreEqual(1, token.Line);
            token = lexer.GetNextToken();
            Assert.AreEqual(Token.Types.KwVar, token.Type);
            Assert.AreEqual(12, token.Column);
            Assert.AreEqual(1, token.Line);
        }

        [TestMethod]
        public void TestTokensOnMultipleLines()
        {
            Scanner lexer = CreateStringLexer("int\nstring var");
            Token token = lexer.GetNextToken();
            Assert.AreEqual(Token.Types.KwInt, token.Type);
            Assert.AreEqual(1, token.Column);
            Assert.AreEqual(1, token.Line);
            token = lexer.GetNextToken();
            Assert.AreEqual(Token.Types.KwString, token.Type);
            Assert.AreEqual(1, token.Column);
            Assert.AreEqual(2, token.Line);
            token = lexer.GetNextToken();
            Assert.AreEqual(Token.Types.KwVar, token.Type);
            Assert.AreEqual(8, token.Column);
            Assert.AreEqual(2, token.Line);
        }

        [TestMethod]
        public void TestSkippingWhitespace()
        {
            Scanner lexer = CreateStringLexer("int \n\tvar");
            Token token = lexer.GetNextToken();
            Assert.AreEqual(Token.Types.KwInt, token.Type);
            Assert.AreEqual(1, token.Column);
            Assert.AreEqual(1, token.Line);
            token = lexer.GetNextToken();
            Assert.AreEqual(Token.Types.KwVar, token.Type);
            Assert.AreEqual(2, token.Column);
            Assert.AreEqual(2, token.Line);
        }

        [TestMethod]
        public void TestStringEscapes()
        {
            Scanner lexer = CreateStringLexer("\"asd \\\"\\n\\t\\\\\"");
            Assert.AreEqual("asd \"\n\t\\", lexer.GetNextToken().Content);
        }

        [TestMethod]
        public void TestUnknownStringEscape()
        {
            Scanner lexer = CreateStringLexer("\"asd \\a\"");
            try
            {
                lexer.GetNextToken();
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
            Scanner lexer = CreateStringLexer("\"asd\n");
            try
            {
                lexer.GetNextToken();
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
            Scanner lexer = CreateStringLexer("@");
            try
            {
                lexer.GetNextToken();
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
            Scanner lexer = CreateStringLexer("var//int\nstring");
            Assert.AreEqual(Token.Types.KwVar, lexer.GetNextToken().Type);
            Assert.AreEqual(Token.Types.KwString, lexer.GetNextToken().Type);
        }

        [TestMethod]
        public void Scanner_WithSingleLineCommentAndWhitespace_SkipsCommentedCode()
        {
            Scanner lexer = CreateStringLexer("var //int \n \n  string");
            Assert.AreEqual(Token.Types.KwVar, lexer.GetNextToken().Type);
            Assert.AreEqual(Token.Types.KwString, lexer.GetNextToken().Type);
        }

        [TestMethod]
        public void Scanner_WithSingleLineCommentAtBeginningOfLine_SkipsCommentedCode()
        {
            Scanner lexer = CreateStringLexer("//int\nstring");
            Assert.AreEqual(Token.Types.KwString, lexer.GetNextToken().Type);
        }

        [TestMethod]
        public void Scanner_WithMultipleSingleLineComments_SkipsCommentedCode()
        {
            Scanner lexer = CreateStringLexer("var //int//123\n  //456\nstring");
            Assert.AreEqual(Token.Types.KwVar, lexer.GetNextToken().Type);
            Assert.AreEqual(Token.Types.KwString, lexer.GetNextToken().Type);
        }

        [TestMethod]
        public void Scanner_WithMultilineCommentOnOneLine_SkipsCommentedCode()
        {
            Scanner lexer = CreateStringLexer("var/*int*/\nstring");
            Assert.AreEqual(Token.Types.KwVar, lexer.GetNextToken().Type);
            Assert.AreEqual(Token.Types.KwString, lexer.GetNextToken().Type);
        }

        [TestMethod]
        public void Scanner_WithMultilineCommentOnMultipleLines_SkipsCommentedCode()
        {
            Scanner lexer = CreateStringLexer("var/*int\n123\nabc*/\nstring");
            Assert.AreEqual(Token.Types.KwVar, lexer.GetNextToken().Type);
            Assert.AreEqual(Token.Types.KwString, lexer.GetNextToken().Type);
        }

        [TestMethod]
        public void Scanner_WithNestedMultilineComments_SkipsCommentedCode()
        {
            Scanner lexer = CreateStringLexer("var/*int/*123*/abc*/\nstring");
            Assert.AreEqual(Token.Types.KwVar, lexer.GetNextToken().Type);
            Assert.AreEqual(Token.Types.KwString, lexer.GetNextToken().Type);
        }

        [TestMethod]
        public void Scanner_WithMultilineCommentAndWhitespace_SkipsCommentedCode()
        {
            Scanner lexer = CreateStringLexer("var \n/*int*/ \n \n  string");
            Assert.AreEqual(Token.Types.KwVar, lexer.GetNextToken().Type);
            Assert.AreEqual(Token.Types.KwString, lexer.GetNextToken().Type);
        }

        [TestMethod]
        public void Scanner_WithMultilineCommentEndingAtFileEnd_SkipsCommentedCode()
        {
            Scanner lexer = CreateStringLexer("var/*int\nstring*/");
            Assert.AreEqual(Token.Types.KwVar, lexer.GetNextToken().Type);
            Assert.IsNull(lexer.GetNextToken());
        }

        [TestMethod]
        public void Scanner_WithMultilineCommentAtBeginningOfLine_SkipsCommentedCode()
        {
            Scanner lexer = CreateStringLexer("/*int*/string");
            Assert.AreEqual(Token.Types.KwString, lexer.GetNextToken().Type);
        }

        [TestMethod]
        public void Scanner_WithMultilineCommentBetweenCodeOnSameLine_SkipsCommentedCode()
        {
            Scanner lexer = CreateStringLexer("var /* int */ string");
            Assert.AreEqual(Token.Types.KwVar, lexer.GetNextToken().Type);
            Assert.AreEqual(Token.Types.KwString, lexer.GetNextToken().Type);
        }

        [TestMethod]
        public void Scanner_WithUnclosedMultilineComment_ThrowsException()
        {
            Scanner lexer = CreateStringLexer("var/*int\nstring");
            Assert.AreEqual(Token.Types.KwVar, lexer.GetNextToken().Type);
            try
            {
                lexer.GetNextToken();
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
            Scanner lexer = CreateStringLexer("var /* int */ //123\n/*321*/string");
            Assert.AreEqual(Token.Types.KwVar, lexer.GetNextToken().Type);
            Assert.AreEqual(Token.Types.KwString, lexer.GetNextToken().Type);
        }

        [TestMethod]
        public void Scanner_WithSingleLineCommentInsideMultiline_SkipsCommentedCode()
        {
            Scanner lexer = CreateStringLexer("var /* //int */string");
            Assert.AreEqual(Token.Types.KwVar, lexer.GetNextToken().Type);
            Assert.AreEqual(Token.Types.KwString, lexer.GetNextToken().Type);
        }

        [TestMethod]
        public void Scanner_WithMultilineCommentInsideSingleLine_SkipsCommentedCode()
        {
            Scanner lexer = CreateStringLexer("var ///*123\nstring");
            Assert.AreEqual(Token.Types.KwVar, lexer.GetNextToken().Type);
            Assert.AreEqual(Token.Types.KwString, lexer.GetNextToken().Type);
        }

        public Scanner CreateStringLexer(string source)
        {
            SourceReader reader = new SourceReader(new System.IO.StringReader(source));
            return new Scanner(reader);
        }
    }
}
