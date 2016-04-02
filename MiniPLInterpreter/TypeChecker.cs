using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lexer
{
    class TypeChecker
    {
        private Dictionary<Operator, NodeTypes> intTypeBindings;
        private Dictionary<Operator, NodeTypes> boolTypeBindings;
        private Dictionary<Operator, NodeTypes> stringTypeBindings;
        private Dictionary<NodeTypes, Dictionary<Operator, NodeTypes>> typeBindings;

        public TypeChecker()
        {
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
        public NodeTypes TypeCheck(NodeTypes type1, NodeTypes type2, Operator op)
        {
            if (type1 != type2)
                throw new Exception(type1 + " !=" + type2);
            var opBindings = typeBindings[type1];
            return opBindings[op];
        }

        internal NodeTypes TypeCheck(NodeTypes nodeType, Operator op)
        {
            var opBindings = typeBindings[nodeType];
            return opBindings[op];
        }
    }
}
