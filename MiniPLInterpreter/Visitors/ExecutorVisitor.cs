using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frontend;

namespace Interpreter
{
    using BinEvaluatorFunc = Func<object, object, object>;
    using UnaryEvaluatorFunc = Func<object, object>;

    class ExecutorVisitor : DefaultVisitor
    {
        private SymbolTable SymbolTable;
        private ExpressionEvaluator Evaluator;
        private StmtList RootStmtList;

        public ExecutorVisitor(SymbolTable symbolTable, StmtList statements)
        {
            SymbolTable = symbolTable;
            Evaluator = new ExpressionEvaluator();
            RootStmtList = statements;
        }

        public void Execute()
        {
            RootStmtList.Accept(this);
        }

        public override void Visit(StmtList stmtList)
        {
            foreach (Statement statement in stmtList.Statements)
            {
                statement.Accept(this);
            }
        }

        public override void Visit(DeclarationStmt declarationStmt)
        {
            IdentifierExpr id = declarationStmt.Identifier;
            if (declarationStmt.AssignmentExpr != null)
            {
                declarationStmt.AssignmentExpr.Accept(this);
                SymbolTable.GetSymbol(id.IdentifierName).Value = declarationStmt.AssignmentExpr.ExprValue;
            }
        }

        public override void Visit(AssignmentStmt assignmentStmt)
        {
            assignmentStmt.AssignmentExpr.Accept(this);
            SymbolTable.SetSymbolValue(assignmentStmt.Identifier.IdentifierName, assignmentStmt.AssignmentExpr.ExprValue);

        }

        public override void Visit(BinaryExpr binaryExpr)
        {
            binaryExpr.Left.Accept(this);
            binaryExpr.Right.Accept(this);
            binaryExpr.ExprValue = Evaluator.EvaluateExpression(binaryExpr.Left, binaryExpr.Right, binaryExpr.Op);
        }

        public override void Visit(UnaryExpr unaryExpr)
        {
            unaryExpr.Expr.Accept(this);
            unaryExpr.ExprValue = Evaluator.EvaluateExpression(unaryExpr.Expr, unaryExpr.Op);
        }

        public override void Visit(AssertStmt assertStmt)
        {
            assertStmt.AssertExpr.Accept(this);
            if ((bool) assertStmt.AssertExpr.ExprValue != true)
            {
                throw new RuntimeException(String.Format("Assertion failed at line {0} column {1}.",
                    assertStmt.Line, assertStmt.Column));
            }
        }

        public override void Visit(ForStmt forStmt)
        {
            forStmt.StartExpr.Accept(this);
            forStmt.EndExpr.Accept(this);
            forStmt.LoopVar.Accept(this);
            Symbol loopSymbol = SymbolTable.GetSymbol(forStmt.LoopVar.IdentifierName);
            
            for(int i = (int)forStmt.StartExpr.ExprValue; i <= (int)forStmt.EndExpr.ExprValue; i++)
            {
                loopSymbol.Value = i;
                forStmt.Body.Accept(this);
            }
        }

        public override void Visit(PrintStmt printStmt)
        {
            printStmt.PrintExpr.Accept(this);
            Console.Write(printStmt.PrintExpr.ExprValue.ToString());
        }

        public override void Visit(ReadStmt readStmt)
        {
            string userInput = Console.ReadLine();
            Symbol inputSymbol = SymbolTable.GetSymbol(readStmt.Variable.IdentifierName);
            switch (readStmt.Variable.Type)
            {
                case ExprType.IntType: inputSymbol.Value = int.Parse(userInput); break;
                case ExprType.StringType: inputSymbol.Value = userInput; break;
                case ExprType.BoolType:
                    userInput = userInput.ToLower();
                    if (userInput == "false" || userInput == "0")
                        inputSymbol.Value = false;
                    else
                        inputSymbol.Value = true;
                    break;
            }
        }

        public override void Visit(IdentifierExpr identifierNode)
        {
            identifierNode.ExprValue = SymbolTable.GetSymbolValue(identifierNode.IdentifierName);
        }
    }

    class ExpressionEvaluator
    {
        
        private Dictionary<Operator, BinEvaluatorFunc> BinaryIntEvaluators;
        private Dictionary<Operator, BinEvaluatorFunc> BinaryStringEvaluators;
        private Dictionary<Operator, BinEvaluatorFunc> BinaryBoolEvaluators;
        private Dictionary<Operator, UnaryEvaluatorFunc> UnaryBoolEvaluators;
        private Dictionary<ExprType, Dictionary<Operator, BinEvaluatorFunc>> BinaryExprEvaluators;
        private Dictionary<ExprType, Dictionary<Operator, UnaryEvaluatorFunc>> UnaryExprEvaluators;

        public ExpressionEvaluator()
        {
            BinaryIntEvaluators = new Dictionary<Operator, BinEvaluatorFunc>()
            {
                { Operator.Plus, (var1, var2) => (int)var1 + (int)var2 },
                { Operator.Minus, (var1, var2) => (int)var1 - (int)var2 },
                { Operator.Times, (var1, var2) => (int)var1 * (int)var2 },
                { Operator.Divide, (var1, var2) => (int)var1 / (int)var2 },
                { Operator.Equals, (var1, var2) => (int)var1 == (int)var2 },
                { Operator.Less, (var1, var2) => (int)var1 < (int)var2 }
            };
            BinaryStringEvaluators = new Dictionary<Operator, BinEvaluatorFunc>()
            {
                { Operator.Plus, (var1, var2) => (string)var1 + (string)var2 },
                { Operator.Equals, (var1, var2) => (string)var1 == (string)var2 },
                { Operator.Less, (var1, var2) => string.Compare((string)var1, (string)var2) < 0 }
            };
            BinaryBoolEvaluators = new Dictionary<Operator, BinEvaluatorFunc>()
            {
                { Operator.Equals, (var1, var2) => (bool)var1 == (bool)var2 },
                { Operator.And, (var1, var2) => (bool)var1 && (bool)var2 },
                { Operator.Less, (var1, var2) => !(bool)var1 && (bool)var2 }
            };
            BinaryExprEvaluators = new Dictionary<ExprType, Dictionary<Operator, BinEvaluatorFunc>>()
            {
                { ExprType.IntType, BinaryIntEvaluators },
                { ExprType.StringType, BinaryStringEvaluators },
                { ExprType.BoolType, BinaryBoolEvaluators }
            };
            UnaryBoolEvaluators = new Dictionary<Operator, UnaryEvaluatorFunc>()
            {
                { Operator.Not, var => !(bool)var }
            };
            UnaryExprEvaluators = new Dictionary<ExprType, Dictionary<Operator, UnaryEvaluatorFunc>>
            {
                { ExprType.BoolType, UnaryBoolEvaluators }
            };
        }
        public object EvaluateExpression(Expression expression1, Expression expression2, Operator op)
        {
            return BinaryExprEvaluators[expression1.Type][op](expression1.ExprValue, expression2.ExprValue);
        }

        public object EvaluateExpression(Expression expr, Operator op)
        {
            return UnaryExprEvaluators[expr.Type][op](expr.ExprValue);
        }
    }

    public class RuntimeException : Exception
    {
        public RuntimeException(string message) : base(message) { }
    }
}
