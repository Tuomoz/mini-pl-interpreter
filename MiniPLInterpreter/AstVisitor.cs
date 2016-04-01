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
        public void Visit(IdentifierExpr identifierNode)
        {
            DefaultVisit(identifierNode);
        }

        public void Visit(AssignmentStmt assignmentStmt)
        {
            DefaultVisit(assignmentStmt);
        }

        public void Visit(ReadStmt readStmt)
        {
            DefaultVisit(readStmt);
        }

        public void Visit(BinaryExpr binaryExpr)
        {
            DefaultVisit(binaryExpr);
        }

        public void Visit(IntLiteralExpr intLiteralExpr)
        {
            DefaultVisit(intLiteralExpr);
        }

        public void Visit(StringLiteralExpr stringLiteralExpr)
        {
            DefaultVisit(stringLiteralExpr);
        }

        public void Visit(PrintStmt printStmt)
        {
            DefaultVisit(printStmt);
        }

        public void Visit(UnaryExpr unaryExpr)
        {
            DefaultVisit(unaryExpr);
        }

        public void Visit(AssertStmt assertStmt)
        {
            DefaultVisit(assertStmt);
        }

        public void Visit(ForStmt forStmt)
        {
            DefaultVisit(forStmt);
        }

        public void Visit(TypeNode typeNode)
        {
            DefaultVisit(typeNode);
        }

        public void Visit(DeclarationStmt declarationStmt)
        {
            DefaultVisit(declarationStmt);
        }

        public void Visit(StmtList stmtList)
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
            PrintNode("Int " + intLiteralExpr.Value.ToString(), true);
        }

        public void Visit(StringLiteralExpr stringLiteralExpr)
        {
            PrintNode("String " + stringLiteralExpr.Value, true);
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
            unaryExpr.Exp.Accept(this);
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
}
