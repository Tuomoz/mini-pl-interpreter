using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interpreter;

namespace Frontend
{
    class TypeCheckerVisitor : DefaultVisitor
    {
        private SymbolTable SymbolTable;
        private TypeChecker Checker;
        private ErrorHandler Errors;
        private StmtList RootStmtList;

        public TypeCheckerVisitor(ErrorHandler errors, StmtList statements)
        {
            Errors = errors;
            SymbolTable = new SymbolTable();
            Checker = new TypeChecker(errors);
            RootStmtList = statements;
        }

        public SymbolTable TypeCheck()
        {
            RootStmtList.Accept(this);
            return SymbolTable;
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
            if (!SymbolTable.AddSymbol(id.IdentifierName, declarationStmt.Type.Type))
            {
                Errors.AddError(String.Format("Already declared variable {0} at line {1} column {2}.",
                    id.IdentifierName, id.Line, id.Column), ErrorTypes.SemanticError);
            }
            if (declarationStmt.AssignmentExpr != null)
            {
                Expression assignment = declarationStmt.AssignmentExpr;
                assignment.Accept(this);
                if (assignment.NodeType != declarationStmt.Type.Type)
                {
                    Errors.AddError(String.Format("Can't assign expression of type {0} in variable {1} of type {2} at line {3} column {4}.",
                        assignment.NodeType, id.IdentifierName, declarationStmt.Type, declarationStmt.Line, declarationStmt.Column), ErrorTypes.SemanticError);
                }
            }
            declarationStmt.Identifier.NodeType = declarationStmt.Type.Type;
        }

        public override void Visit(AssignmentStmt assignmentStmt)
        {
            IdentifierExpr id = assignmentStmt.Identifier;
            id.Accept(this);
            Expression assignment = assignmentStmt.AssignmentExpr;
            assignment.Accept(this);
            if (id.NodeType != assignment.NodeType)
            {
                Errors.AddError(String.Format("Can't assign expression of type {0} in variable {1} of type {2} at line {3} column {4}.",
                    assignment.NodeType, id.IdentifierName, id.NodeType, assignmentStmt.Line, assignmentStmt.Column), ErrorTypes.SemanticError);
            }

        }

        public override void Visit(BinaryExpr binaryExpr)
        {
            binaryExpr.Left.Accept(this);
            binaryExpr.Right.Accept(this);
            binaryExpr.NodeType = Checker.TypeCheck(binaryExpr.Left, binaryExpr.Right, binaryExpr.Op);
        }

        public override void Visit(UnaryExpr unaryExpr)
        {
            unaryExpr.Expr.Accept(this);
            unaryExpr.NodeType = Checker.TypeCheck(unaryExpr.Expr, unaryExpr.Op);
        }

        public override void Visit(AssertStmt assertStmt)
        {
            Expression assert = assertStmt.AssertExpr;
            assert.Accept(this);
            if (assert.NodeType != ExprType.BoolType)
            {
                Errors.AddError(String.Format("Assertion expression type {0} illegal at line {1} column {2}.",
                    assert.NodeType, assertStmt.Line, assertStmt.Column), ErrorTypes.SemanticError);
            }
        }

        public override void Visit(ForStmt forStmt)
        {
            Expression start = forStmt.StartExpr;
            Expression end = forStmt.EndExpr;
            IdentifierExpr loopvar = forStmt.LoopVar;
            start.Accept(this);
            end.Accept(this);
            loopvar.Accept(this);
            if (loopvar.NodeType != ExprType.IntType)
            {
                Errors.AddError(String.Format("For loop variable {0} of type {0} illegal at line {1} column {2}.",
                    loopvar.IdentifierName, loopvar.NodeType, loopvar.Line, loopvar.Column), ErrorTypes.SemanticError);
            }
            if (start.NodeType != ExprType.IntType || end.NodeType != ExprType.IntType)
            {
                Errors.AddError(String.Format("For loop expressions must be of type int at line {0} column {1}.",
                    forStmt.Line, forStmt.Column), ErrorTypes.SemanticError);
            }
            forStmt.Body.Accept(this);
        }

        public override void Visit(PrintStmt printStmt)
        {
            printStmt.PrintExpr.Accept(this);
        }

        public override void Visit(ReadStmt readStmt)
        {
            readStmt.Variable.Accept(this);
        }

        public override void Visit(IdentifierExpr identifierNode)
        {
            ExprType? idType = SymbolTable.GetSymbolType(identifierNode.IdentifierName);
            if (idType.HasValue)
            {
                identifierNode.NodeType = idType.Value;
            }
            else
            {
                Errors.AddError(String.Format("Undeclared variable {0} at line {1} column {2}.",
                    identifierNode.IdentifierName, identifierNode.Line, identifierNode.Column), ErrorTypes.SemanticError);
            }
        }
    }
}
