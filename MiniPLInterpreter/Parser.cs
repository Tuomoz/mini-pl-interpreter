namespace Lexer
{
    class Parser
    {
        private Scanner Scanner;
        private Token CurrentToken;

        public Parser(Scanner scanner)
        {
            Scanner = scanner;
            CurrentToken = Scanner.GetNextToken();
        }

        public void Parse()
        {
            ParseProgram();
        }

        private void NextToken()
        {
            CurrentToken = Scanner.GetNextToken();
        }

        private bool Accept(Token.Types excepted)
        {
            if (CurrentToken.Type == excepted)
            {
                NextToken();
                return true;
            }
            return false;
        }

        private void Match(Token.Types excepted)
        {
            if (CurrentToken.Type != excepted)
            {
                throw new System.FormatException(CurrentToken.Type.ToString());
            }
            NextToken();
        }

        private void ParseProgram()
        {
            ParseStatement();
            Match(Token.Types.LineTerm);
            ParseStatements();
        }

        private void ParseStatements()
        {
            if (CurrentToken != null)
            {
                ParseStatement();
                Match(Token.Types.LineTerm);
                ParseStatements();
            }
        }

        private void ParseStatement()
        {
            if (Accept(Token.Types.KwVar))
            {
                Match(Token.Types.Identifier);
                Match(Token.Types.Colon);
                ParseType();
                if (Accept(Token.Types.OpAssignment))
                {
                    ParseExpression();
                }
            }
            else if (Accept(Token.Types.Identifier))
            {
                Match(Token.Types.OpAssignment);
                ParseExpression();
            }
            else if (Accept(Token.Types.KwFor))
            {
                Match(Token.Types.Identifier);
                Match(Token.Types.KwIn);
                ParseExpression();
                Match(Token.Types.OpRange);
                ParseExpression();
                Match(Token.Types.KwDo);
                ParseStatements();
                Match(Token.Types.KwEnd);
                Match(Token.Types.KwFor);
            }
            else if (Accept(Token.Types.KwRead))
            {
                Match(Token.Types.Identifier);
            }
            else if (Accept(Token.Types.KwPrint))
            {
                ParseExpression();
            }
            else if (Accept(Token.Types.KwAssert))
            {
                Match(Token.Types.LParen);
                ParseExpression();
                Match(Token.Types.RParen);
            }
        }

        private void ParseExpression()
        {
            if (Accept(Token.Types.OpNot))
            {
                ParseFactor();
            }
            else
            {
                ParseTerm();
                ParseTermTail();
            }
        }

        private void ParseFactor()
        {
            if (CurrentToken.Type == Token.Types.IntLiteral
                || CurrentToken.Type == Token.Types.StringLiteral
                || CurrentToken.Type == Token.Types.Identifier)
            {
                NextToken();
            }
            else if (Accept(Token.Types.LParen))
            {
                ParseExpression();
                Match(Token.Types.RParen);
            }
        }

        private void ParseTermTail()
        {
            if (CurrentToken.Type == Token.Types.OpPlus
                || CurrentToken.Type == Token.Types.OpMinus)
            {
                NextToken();
                ParseTerm();
                ParseTermTail();
            }
        }

        private void ParseTerm()
        {
            ParseFactor();
            ParseFactorTail();
        }

        private void ParseFactorTail()
        {
            if (CurrentToken.Type == Token.Types.OpMultiply 
                || CurrentToken.Type == Token.Types.OpDivide)
            {
                NextToken();
                ParseFactor();
                ParseFactorTail();
            }
        }

        private void ParseType()
        {
            if (CurrentToken.Type == Token.Types.KwInt
                || CurrentToken.Type == Token.Types.KwString
                || CurrentToken.Type == Token.Types.KwBool)
            {
                NextToken();
            }
        }
    }
}
