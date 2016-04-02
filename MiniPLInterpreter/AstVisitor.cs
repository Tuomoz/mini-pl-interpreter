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
        public SymbolTable SymbolTable = new SymbolTable();
        private TypeChecker Checker = new TypeChecker();

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
            SymbolTable.AddSymbol(id.IdentifierName, declarationStmt.Type.Type);
            if (declarationStmt.AssignmentExpr != null)
            {
                declarationStmt.AssignmentExpr.Accept(this);
                if (declarationStmt.AssignmentExpr.NodeType != declarationStmt.Type.Type)
                    throw new Exception("Wrong type");
            }
            declarationStmt.Identifier.NodeType = declarationStmt.Type.Type;
        }

        public override void Visit(AssignmentStmt assignmentStmt)
        {
            assignmentStmt.Identifier.Accept(this);
            assignmentStmt.AssignmentExpr.Accept(this);
            if (assignmentStmt.Identifier.NodeType != assignmentStmt.AssignmentExpr.NodeType)
                throw new Exception("Wrong type");

        }

        public override void Visit(BinaryExpr binaryExpr)
        {
            binaryExpr.Left.Accept(this);
            binaryExpr.Right.Accept(this);
            NodeTypes leftType = binaryExpr.Left.NodeType, rightType = binaryExpr.Right.NodeType;
            binaryExpr.NodeType = Checker.TypeCheck(leftType, rightType, binaryExpr.Op);
        }

        public override void Visit(UnaryExpr unaryExpr)
        {
            unaryExpr.Expr.Accept(this);
            unaryExpr.NodeType = Checker.TypeCheck(unaryExpr.Expr.NodeType, unaryExpr.Op);
        }

        public override void Visit(AssertStmt assertStmt)
        {
            assertStmt.AssertExpr.Accept(this);
            if (assertStmt.AssertExpr.NodeType != NodeTypes.BoolType)
                throw new Exception("Wrong type");
        }

        public override void Visit(ForStmt forStmt)
        {
            forStmt.StartExpr.Accept(this);
            forStmt.EndExpr.Accept(this);
            forStmt.LoopVar.Accept(this);
            if (forStmt.LoopVar.NodeType != NodeTypes.IntType
                || forStmt.StartExpr.NodeType != NodeTypes.IntType
                || forStmt.EndExpr.NodeType != NodeTypes.IntType)
                throw new Exception("Wrong type");
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
            identifierNode.NodeType = SymbolTable.GetSymbolType(identifierNode.IdentifierName);
        }
    }
}
