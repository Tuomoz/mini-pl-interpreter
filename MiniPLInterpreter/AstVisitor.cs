using Interpreter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lexer
{
    interface IAstVisitor
    {
        void Visit(StmtList stmtList);
        void Visit(DeclarationStmt declarationStmt);
        void Visit(IdentifierExpr identifierNode);
        void Visit(TypeNode typeNode);
        void Visit(AssignmentStmt assignmentStmt);
        void Visit(ForStmt forStmt);
        void Visit(ReadStmt readStmt);
        void Visit(AssertStmt assertStmt);
        void Visit(BinaryExpr binaryExpr);
        void Visit(UnaryExpr unaryExpr);
        void Visit(IntLiteralExpr intLiteralExpr);
        void Visit(PrintStmt printStmt);
        void Visit(StringLiteralExpr stringLiteralExpr);
    }

    abstract class DefaultVisitor : IAstVisitor
    {
        public virtual void Visit(IdentifierExpr identifierNode)
        {
            DefaultVisit(identifierNode);
        }

        public virtual void Visit(AssignmentStmt assignmentStmt)
        {
            DefaultVisit(assignmentStmt);
        }

        public virtual void Visit(ReadStmt readStmt)
        {
            DefaultVisit(readStmt);
        }

        public virtual void Visit(BinaryExpr binaryExpr)
        {
            DefaultVisit(binaryExpr);
        }

        public virtual void Visit(IntLiteralExpr intLiteralExpr)
        {
            DefaultVisit(intLiteralExpr);
        }

        public virtual void Visit(StringLiteralExpr stringLiteralExpr)
        {
            DefaultVisit(stringLiteralExpr);
        }

        public virtual void Visit(PrintStmt printStmt)
        {
            DefaultVisit(printStmt);
        }

        public virtual void Visit(UnaryExpr unaryExpr)
        {
            DefaultVisit(unaryExpr);
        }

        public virtual void Visit(AssertStmt assertStmt)
        {
            DefaultVisit(assertStmt);
        }

        public virtual void Visit(ForStmt forStmt)
        {
            DefaultVisit(forStmt);
        }

        public virtual void Visit(TypeNode typeNode)
        {
            DefaultVisit(typeNode);
        }

        public virtual void Visit(DeclarationStmt declarationStmt)
        {
            DefaultVisit(declarationStmt);
        }

        public virtual void Visit(StmtList stmtList)
        {
            DefaultVisit(stmtList);
        }

        public void DefaultVisit(AstNode node) { }
    }

    class AstPrinterVisitor : IAstVisitor
    {
        int TreeLevel = -1;

        public void Visit(IdentifierExpr identifierNode)
        {
            PrintNode("Id " + identifierNode.IdentifierName, true);
        }

        public void Visit(AssignmentStmt assignmentStmt)
        {
            TreeLevel++;
            PrintNode("AssignmentStm");
            assignmentStmt.Identifier.Accept(this);
            assignmentStmt.AssignmentExpr.Accept(this);
            TreeLevel--;
        }

        public void Visit(ReadStmt readStmt)
        {
            TreeLevel++;
            PrintNode("ReadStm");
            readStmt.Variable.Accept(this);
            TreeLevel--;
        }

        public void Visit(BinaryExpr binaryExpr)
        {
            TreeLevel++;
            PrintNode("binaryExpr");
            PrintNode(binaryExpr.Op.ToString(), true);
            binaryExpr.Left.Accept(this);
            binaryExpr.Right.Accept(this);
            TreeLevel--;
        }

        public void Visit(IntLiteralExpr intLiteralExpr)
        {
            PrintNode("Int " + intLiteralExpr.ExprValue.ToString(), true);
        }

        public void Visit(StringLiteralExpr stringLiteralExpr)
        {
            PrintNode("String " + stringLiteralExpr.ExprValue, true);
        }

        public void Visit(PrintStmt printStmt)
        {
            TreeLevel++;
            PrintNode("PrintStm");
            printStmt.PrintExpr.Accept(this);
            TreeLevel--;
        }

        public void Visit(StmtList stmtList)
        {
            TreeLevel++;
            PrintNode("StmtList");
            for (int i = 0; i < stmtList.Statements.Count; i++)
            {
                stmtList.Statements[i].Accept(this);
            }
            TreeLevel--;
        }

        public void Visit(UnaryExpr unaryExpr)
        {
            TreeLevel++;
            PrintNode("UnaryExpr");
            PrintNode(unaryExpr.Op.ToString(), true);
            unaryExpr.Expr.Accept(this);
            TreeLevel--;
        }

        public void Visit(AssertStmt assertStmt)
        {
            TreeLevel++;
            PrintNode("AssertStm");
            assertStmt.AssertExpr.Accept(this);
            TreeLevel--;
        }

        public void Visit(ForStmt forStmt)
        {
            TreeLevel++;
            PrintNode("ForStm");
            forStmt.LoopVar.Accept(this);
            forStmt.StartExpr.Accept(this);
            forStmt.EndExpr.Accept(this);
            forStmt.Body.Accept(this);
            TreeLevel--;
        }

        public void Visit(TypeNode typeNode)
        {
            PrintNode("Type " + typeNode.Type.ToString(), true);
        }

        public void Visit(DeclarationStmt declarationStmt)
        {
            TreeLevel++;
            PrintNode("Declaration");
            PrintNode("Name " + declarationStmt.Identifier.IdentifierName, true);
            declarationStmt.Type.Accept(this);
            if (declarationStmt.AssignmentExpr != null)
                declarationStmt.AssignmentExpr.Accept(this);
            TreeLevel--;
        }

        private void PrintNode(string content, bool indent = false)
        {
            StringBuilder line = new StringBuilder();
            line.Append(' ', TreeLevel * 2);
            if (indent) line.Append("  ");
            line.Append(content);
            Console.WriteLine(line.ToString());
        }

        public void Visit(AstNode node)
        {
            PrintNode(node.ToString(), true);
        }
    }

    class TypeCheckerVisitor : DefaultVisitor
    {
        public SymbolTable SymbolTable;
        private TypeChecker Checker;
        private ErrorHandler Errors;

        public TypeCheckerVisitor(ErrorHandler errors)
        {
            SymbolTable = new SymbolTable();
            Checker = new TypeChecker(errors);
            Errors = errors;
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
            if (assert.NodeType != NodeTypes.BoolType)
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
            if (loopvar.NodeType != NodeTypes.IntType)
            {
                Errors.AddError(String.Format("For loop variable {0} of type {0} illegal at line {1} column {2}.",
                    loopvar.IdentifierName, loopvar.NodeType, loopvar.Line, loopvar.Column), ErrorTypes.SemanticError);
            }
            if (start.NodeType != NodeTypes.IntType || end.NodeType != NodeTypes.IntType)
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
            NodeTypes? idType = SymbolTable.GetSymbolType(identifierNode.IdentifierName);
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
