using Interpreter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frontend
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
}
