using Interpreter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frontend
{
    class TypeChecker
    {
        private ErrorHandler Errors;
        private Dictionary<Operator, ExprType> intTypeBindings;
        private Dictionary<Operator, ExprType> boolTypeBindings;
        private Dictionary<Operator, ExprType> stringTypeBindings;
        private Dictionary<ExprType, Dictionary<Operator, ExprType>> typeBindings;

        public TypeChecker(ErrorHandler errors)
        {
            Errors = errors;
            intTypeBindings = new Dictionary<Operator, ExprType>()
            {
                { Operator.Plus, ExprType.IntType },
                { Operator.Minus, ExprType.IntType },
                { Operator.Divide, ExprType.IntType },
                { Operator.Times, ExprType.IntType },
                { Operator.Equals, ExprType.BoolType },
                { Operator.Less, ExprType.BoolType }
            };
            boolTypeBindings = new Dictionary<Operator, ExprType>()
            {
                { Operator.Equals, ExprType.BoolType },
                { Operator.And, ExprType.BoolType },
                { Operator.Not, ExprType.BoolType },
                { Operator.Less, ExprType.BoolType }
            };
            stringTypeBindings = new Dictionary<Operator, ExprType>()
            {
                { Operator.Plus, ExprType.StringType },
                { Operator.Equals, ExprType.BoolType },
                { Operator.Less, ExprType.BoolType }
            };
            typeBindings = new Dictionary<ExprType, Dictionary<Operator, ExprType>>()
            {
                { ExprType.IntType, intTypeBindings },
                { ExprType.BoolType, boolTypeBindings },
                { ExprType.StringType, stringTypeBindings }
            };
        }
        public ExprType TypeCheck(Expression expr1, Expression expr2, Operator op)
        {
            if (expr1.Type == ExprType.VoidType || expr2.Type == ExprType.VoidType)
                return ExprType.VoidType;
            var opBindings = typeBindings[expr1.Type];
            if (opBindings.ContainsKey(op) && expr1.Type == expr2.Type)
            {
                return opBindings[op];
            }
            else
            {
                Errors.AddError(String.Format("Can't apply operator {0} on types {1} and {2} at line {3} column {4}.",
                    op, expr1.Type, expr2.Type, expr1.Line, expr1.Column), ErrorTypes.SemanticError);
                return ExprType.VoidType;
            }
            
        }

        internal ExprType TypeCheck(Expression expr, Operator op)
        {
            if (expr.Type == ExprType.VoidType)
                return ExprType.VoidType;
            var opBindings = typeBindings[expr.Type];
            if (opBindings.ContainsKey(op))
            {
                return opBindings[op];
            }
            else
            {
                Errors.AddError(String.Format("Can't apply operator {0} on type {1} at line {2} column {3}.",
                    op, expr.Type, expr.Line, expr.Column), ErrorTypes.SemanticError);
                return ExprType.VoidType;
            }
        }
    }
}
