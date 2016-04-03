using Interpreter;
using System;

namespace Lexer
{
    class Parser
    {
        private Scanner Scanner;
        private Token CurrentToken;
        private Token AcceptedToken;
        private ErrorHandler Errors;

        public Parser(Scanner scanner, ErrorHandler errors)
        {
            Scanner = scanner;
            Errors = errors;
            CurrentToken = Scanner.GetNextToken();
        }

        public StmtList Parse()
        {
            return ParseProgram();
        }

        private void NextToken()
        {
            CurrentToken = Scanner.GetNextToken();
        }

        private bool Accept(Token.Types excepted)
        {
            if (CurrentToken.Type == excepted)
            {
                AcceptedToken = CurrentToken;
                NextToken();
                return true;
            }
            return false;
        }

        private bool Accept(Token.Types[] excepted)
        {
            if (Array.IndexOf(excepted, CurrentToken.Type) != -1)
            {
                AcceptedToken = CurrentToken;
                NextToken();
                return true;
            }
            return false;
        }

        private BinaryExpr CreateBinaryExpr(Token token = null)
        {
            if (token == null)
            {
                token = AcceptedToken;
            }
            BinaryExpr expr = new BinaryExpr(token.Line, token.Column);
            switch(token.Type)
            {
                case Token.Types.OpPlus: expr.Op = Operator.Plus; break;
                case Token.Types.OpMinus: expr.Op = Operator.Minus; break;
                case Token.Types.OpMultiply: expr.Op = Operator.Times; break;
                case Token.Types.OpDivide: expr.Op = Operator.Divide; break;
                case Token.Types.OpLess: expr.Op = Operator.Less; break;
                case Token.Types.OpEquals: expr.Op = Operator.Equals; break;
                case Token.Types.OpAnd: expr.Op = Operator.And; break;
            }
            return expr;
        }

        private Token Match(Token.Types excepted)
        {
            if (CurrentToken.Type != excepted)
            {
                if (excepted == Token.Types.LineTerm)
                    return new Token(Token.Types.LineTerm, CurrentToken.Line, CurrentToken.Column);
                throw new ParserException(String.Format("Expected {0}, got {1} instead at line {2} column {3}.",
                    excepted, CurrentToken.Type, CurrentToken.Line, CurrentToken.Column));
            }
            Token match = CurrentToken;
            NextToken();
            return match;
        }

        private StmtList ParseProgram()
        {
            StmtList statements = new StmtList(CurrentToken.Line, CurrentToken.Column);
            ParseStatements(statements);
            return statements;
        }

        private StmtList ParseStatements(StmtList statements)
        {
            if (CurrentToken.Type == Token.Types.EOF || CurrentToken.Type == Token.Types.KwEnd)
            {
                return statements;
            }
            try
            {
                statements.AddStatement(ParseStatement());
                Match(Token.Types.LineTerm);
            }
            catch (ParserException e)
            {
                Errors.AddError(e.Message, ErrorTypes.SyntaxError);
                SkipToNextStatement();
            }
            return ParseStatements(statements);
        }

        private void SkipToNextStatement()
        {
            while (CurrentToken.Type != Token.Types.LineTerm && CurrentToken.Type != Token.Types.EOF)
            {
                NextToken();
            }
            NextToken();
        }

        private Statement ParseStatement()
        {
            if (Accept(Token.Types.KwVar))
            {
                DeclarationStmt declaration = new DeclarationStmt(AcceptedToken.Line, AcceptedToken.Column);
                Token id = Match(Token.Types.Identifier);
                declaration.Identifier = new IdentifierExpr(id.Line, id.Column, id.Content);
                Match(Token.Types.Colon);
                TypeNode typeNode = ParseType();
                declaration.Type = typeNode;
                if (Accept(Token.Types.OpAssignment))
                {
                    declaration.AssignmentExpr = ParseExpression();
                }
                return declaration;
            }
            else if (Accept(Token.Types.Identifier))
            {
                AssignmentStmt statement = new AssignmentStmt(AcceptedToken.Line, AcceptedToken.Column);
                statement.Identifier = new IdentifierExpr(AcceptedToken.Line, AcceptedToken.Column, AcceptedToken.Content);
                Match(Token.Types.OpAssignment);
                statement.AssignmentExpr = ParseExpression();
                return statement;
            }
            else if (Accept(Token.Types.KwFor))
            {
                ForStmt statement = new ForStmt(AcceptedToken.Line, AcceptedToken.Column);
                Token idToken = Match(Token.Types.Identifier);
                statement.LoopVar = new IdentifierExpr(idToken.Line, idToken.Column, idToken.Content);
                Match(Token.Types.KwIn);
                statement.StartExpr = ParseExpression();
                Match(Token.Types.OpRange);
                statement.EndExpr = ParseExpression();
                Match(Token.Types.KwDo);
                statement.Body = ParseStatements(new StmtList(CurrentToken.Line, CurrentToken.Column));
                Match(Token.Types.KwEnd);
                Match(Token.Types.KwFor);
                return statement;
            }
            else if (Accept(Token.Types.KwRead))
            {
                ReadStmt statement = new ReadStmt(AcceptedToken.Line, AcceptedToken.Column);
                Token idToken = Match(Token.Types.Identifier);
                statement.Variable = new IdentifierExpr(idToken.Line, idToken.Column, idToken.Content);
                return statement;
            }
            else if (Accept(Token.Types.KwPrint))
            {
                PrintStmt statement = new PrintStmt(AcceptedToken.Line, AcceptedToken.Column);
                statement.PrintExpr = ParseExpression();
                return statement;
            }
            else if (Accept(Token.Types.KwAssert))
            {
                AssertStmt statement = new AssertStmt(AcceptedToken.Line, AcceptedToken.Column);
                Match(Token.Types.LParen);
                statement.AssertExpr = ParseExpression();
                Match(Token.Types.RParen);
                return statement;
            }
            throw new ParserException(String.Format("Expected statement, got {0} instead at line {1} column {2}.",
                CurrentToken.Type, CurrentToken.Line, CurrentToken.Column));
        }

        private Expression ParseExpression()
        {
            if (Accept(Token.Types.OpNot))
            {
                UnaryExpr unary = new UnaryExpr(AcceptedToken.Line, AcceptedToken.Column);
                unary.Op = Operator.Not;
                unary.Expr = ParseFactor();
                return unary;
            }
            else
            {
                Expression exp = ParseTerm();
                BinaryExpr tail = ParseTermTail(exp);
                if (tail != null)
                {
                    exp = tail;
                }
                BinaryExpr logical = ParseLogical(exp);
                if (logical != null)
                {
                    return logical;
                }
                else
                {
                    return exp;
                }
            }
        }

        private BinaryExpr ParseLogical(Expression left)
        {
            if (Accept(new[] { Token.Types.OpLess, Token.Types.OpEquals, Token.Types.OpAnd }))
            {
                BinaryExpr exp = CreateBinaryExpr();
                exp.Left = left;
                exp.Right = ParseExpression();
                return exp;
            }
            else
            {
                return null;
            }
        }

        private Expression ParseFactor()
        {
            if (Accept(Token.Types.IntLiteral))
            {
                IntLiteralExpr exp = new IntLiteralExpr(AcceptedToken.Line, AcceptedToken.Column);
                exp.ExprValue = int.Parse(AcceptedToken.Content);
                return exp;
            }
            else if (Accept(Token.Types.StringLiteral))
            {
                StringLiteralExpr expr = new StringLiteralExpr(AcceptedToken.Line, AcceptedToken.Column);
                expr.ExprValue = AcceptedToken.Content;
                return expr;
            }
            else if (Accept(Token.Types.Identifier))
            {
                IdentifierExpr node = new IdentifierExpr(AcceptedToken.Line, AcceptedToken.Column, AcceptedToken.Content);
                return node;
            }
            else if (Accept(Token.Types.LParen))
            {
                Expression exp = ParseExpression();
                Match(Token.Types.RParen);
                return exp;
            }
            else throw new ParserException(String.Format("Expected literal or identifier, got {0} instead at line {1} column {2}.", 
                CurrentToken.Type, CurrentToken.Line, CurrentToken.Column));
        }

        private BinaryExpr ParseTermTail(Expression leftExp)
        {
            if (Accept(new[] { Token.Types.OpPlus, Token.Types.OpMinus }))
            {
                BinaryExpr exp = CreateBinaryExpr();
                exp.Left = leftExp;
                exp.Right = ParseTerm();
                BinaryExpr tail = ParseTermTail(exp);
                if (tail != null)
                {
                    return tail;
                }
                else
                {
                    return exp;
                }
            }
            else return null;
        }

        private Expression ParseTerm()
        {
            Expression factor = ParseFactor();
            BinaryExpr exp = ParseFactorTail(factor);
            if (exp != null)
            {
                return exp;
            }
            else
            {
                return factor;
            }
        }

        private BinaryExpr ParseFactorTail(Expression leftExpr)
        {
            if (Accept(new[] { Token.Types.OpMultiply, Token.Types.OpDivide }))
            {
                BinaryExpr exp = CreateBinaryExpr();
                exp.Left = leftExpr;
                exp.Right = ParseFactor();
                BinaryExpr tail = ParseFactorTail(exp);
                if (tail != null)
                {
                    return tail;
                }
                else
                {
                    return exp;
                }
            }
            else return null;
        }

        private TypeNode ParseType()
        {
            if (Accept(Token.Types.KwInt))
            {
                return new TypeNode(CurrentToken.Line, CurrentToken.Column, NodeTypes.IntType);
            }
            else if (Accept(Token.Types.KwString))
            {
                return new TypeNode(CurrentToken.Line, CurrentToken.Column, NodeTypes.StringType);
            }
            else if (Accept(Token.Types.KwBool))
            {
                return new TypeNode(CurrentToken.Line, CurrentToken.Column, NodeTypes.BoolType);
            }
            else
            {
                throw new ParserException(String.Format("Unknown type \"{0}\" at line {1} column {2}.",
                    CurrentToken.Content, CurrentToken.Line, CurrentToken.Column));
            }
        }
    }

    class ParserException : Exception
    {
        public ParserException(string message) : base(message) { }
    }
}
