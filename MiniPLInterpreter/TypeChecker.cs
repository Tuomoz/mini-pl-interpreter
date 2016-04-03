using Interpreter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lexer
{
    class TypeChecker
    {
        private ErrorHandler Errors;
        private Dictionary<Operator, NodeTypes> intTypeBindings;
        private Dictionary<Operator, NodeTypes> boolTypeBindings;
        private Dictionary<Operator, NodeTypes> stringTypeBindings;
        private Dictionary<NodeTypes, Dictionary<Operator, NodeTypes>> typeBindings;

        public TypeChecker(ErrorHandler errors)
        {
            Errors = errors;
            intTypeBindings = new Dictionary<Operator, NodeTypes>()
            {
                { Operator.Plus, NodeTypes.IntType },
                { Operator.Minus, NodeTypes.IntType },
                { Operator.Divide, NodeTypes.IntType },
                { Operator.Times, NodeTypes.IntType },
                { Operator.Equals, NodeTypes.BoolType },
                { Operator.Less, NodeTypes.BoolType }
            };
            boolTypeBindings = new Dictionary<Operator, NodeTypes>()
            {
                { Operator.Equals, NodeTypes.BoolType },
                { Operator.And, NodeTypes.BoolType },
                { Operator.Not, NodeTypes.BoolType },
                { Operator.Less, NodeTypes.BoolType }
            };
            stringTypeBindings = new Dictionary<Operator, NodeTypes>()
            {
                { Operator.Plus, NodeTypes.StringType },
                { Operator.Equals, NodeTypes.BoolType },
                { Operator.Less, NodeTypes.BoolType }
            };
            typeBindings = new Dictionary<NodeTypes, Dictionary<Operator, NodeTypes>>()
            {
                { NodeTypes.IntType, intTypeBindings },
                { NodeTypes.BoolType, boolTypeBindings },
                { NodeTypes.StringType, stringTypeBindings }
            };
        }
        public NodeTypes TypeCheck(Expression expr1, Expression expr2, Operator op)
        {
            var opBindings = typeBindings[expr1.NodeType];
            if (opBindings.ContainsKey(op) && expr1.NodeType == expr2.NodeType)
            {
                return opBindings[op];
            }
            else
            {
                Errors.AddError(String.Format("Can't apply operator {0} on types {1} and {2} at line {3} column {4}.",
                    op, expr1.NodeType, expr2.NodeType, expr1.Line, expr1.Column), ErrorTypes.SemanticError);
                return NodeTypes.VoidType;
            }
            
        }

        internal NodeTypes TypeCheck(Expression expr, Operator op)
        {
            var opBindings = typeBindings[expr.NodeType];
            if (opBindings.ContainsKey(op))
            {
                return opBindings[op];
            }
            else
            {
                Errors.AddError(String.Format("Can't apply operator {0} on type {1} at line {2} column {3}.",
                    op, expr.NodeType, expr.Line, expr.Column), ErrorTypes.SemanticError);
                return NodeTypes.VoidType;
            }
        }
    }
}
