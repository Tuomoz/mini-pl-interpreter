using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lexer
{
    public enum NodeTypes { IntType, StringType, BoolType, VoidType };
    public enum Operator { Plus, Minus, Times, Divide, Less, Equals, And, Not }

    abstract class AstNode
    {
        int Line, Column;

        public AstNode(int line, int column)
        {
            Line = line;
            Column = column;
        }

        public abstract void Accept(IAstVisitor visitor);
    }

    abstract class Statement : AstNode
    {
        public Statement(int line, int column) : base(line, column) { }
    }

    abstract class Expression : AstNode
    {
        public NodeTypes NodeType { get; set; } = NodeTypes.VoidType;
        public object ExprValue;

        public Expression(int line, int column) : base(line, column) { }
    }

    class StmtList : AstNode
    {
        public List<Statement> Statements { get; private set; }

        public StmtList(int line, int column) : base(line, column)
        {
            Statements = new List<Statement>();
        }

        public override void Accept(IAstVisitor visitor)
        {
            visitor.Visit(this);
        }

        public void AddStatement(Statement statement)
        {
            Statements.Add(statement);
        }
    }

    class DeclarationStmt : Statement
    {
        public IdentifierExpr Identifier { get; set; }
        public TypeNode Type { get; set; }
        public Expression AssignmentExpr { get; set; }

        public DeclarationStmt(int line, int column) : base(line, column) { }

        public override void Accept(IAstVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    class IdentifierExpr : Expression
    {
        public string IdentifierName { get; set; }

        public IdentifierExpr(int line, int column, string identifierName) : base(line, column)
        {
            IdentifierName = identifierName;
        }

        public override void Accept(IAstVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    class TypeNode : AstNode
    {
        public NodeTypes Type;

        public TypeNode(int line, int column, NodeTypes type) : base (line, column)
        {
            Type = type;
        }

        public override void Accept(IAstVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    class AssignmentStmt : Statement
    {
        public IdentifierExpr Identifier { get; set; }
        public Expression AssignmentExpr { get; set; }

        public AssignmentStmt(int line, int column) : base(line, column) { }

        public override void Accept(IAstVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    class ForStmt : Statement
    {
        public IdentifierExpr LoopVar { get; set; }
        public Expression StartExpr { get; set; }
        public Expression EndExpr { get; set; }
        public StmtList Body { get; set; }

        public ForStmt(int line, int column) : base(line, column) { }

        public override void Accept(IAstVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    class ReadStmt : Statement
    {
        public IdentifierExpr Variable { get; set; }

        public ReadStmt(int line, int column) : base(line, column) { }

        public override void Accept(IAstVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    class PrintStmt : Statement
    {
        public Expression PrintExpr { get; set; }

        public PrintStmt(int line, int column) : base(line, column) { }

        public override void Accept(IAstVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    class AssertStmt : Statement
    {
        public Expression AssertExpr { get; set; }

        public AssertStmt(int line, int column) : base(line, column) { }

        public override void Accept(IAstVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    class BinaryExpr : Expression
    {
        public Operator Op { get; set; }
        public Expression Left { get; set; }
        public Expression Right { get; set; }

        public BinaryExpr(int line, int column) : base(line, column) { }

        public override void Accept(IAstVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    class UnaryExpr : Expression
    {
        public Operator Op { get; set; }
        public Expression Expr { get; set; }

        public UnaryExpr(int line, int column) : base(line, column) { }

        public override void Accept(IAstVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    class IntLiteralExpr : Expression
    {
        public int Value { get; set; }

        public IntLiteralExpr(int line, int column) : base(line, column) { }

        public override void Accept(IAstVisitor visitor)
        {
            visitor.Visit(this);
            NodeType = NodeTypes.IntType;
        }
    }

    class StringLiteralExpr : Expression
    {
        public string Value { get; set; }

        public StringLiteralExpr(int line, int column) : base(line, column) { }

        public override void Accept(IAstVisitor visitor)
        {
            visitor.Visit(this);
            NodeType = NodeTypes.StringType;
        }
    }
}
