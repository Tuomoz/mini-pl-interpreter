using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lexer
{
    public class SymbolTable
    {
        private Dictionary<string, Symbol> SymbolDic = new Dictionary<string, Symbol>();

        public bool AddSymbol(string name, NodeTypes type, dynamic value = null)
        {
            if (SymbolDic.ContainsKey(name))
            {
                return false;
            }
            SymbolDic.Add(name, new Symbol(name, type, value));
            return true;
        }

        public NodeTypes GetSymbolType(string name)
        {
            return SymbolDic[name].Type;
        }

        public Symbol GetSymbol(string name)
        {
            return SymbolDic[name];
        }

        public void SetSymbolValue(string name, object value)
        {
            SymbolDic[name].Value = value;
        }

        public object GetSymbolValue(string name)
        {
            return SymbolDic[name].Value;
        }
    }

    public class Symbol
    {
        public readonly string Name;
        public readonly Lexer.NodeTypes Type;
        public object Value { get; set; }

        public Symbol(string name, NodeTypes type, object value = null)
        {
            Name = name;
            Type = type;
            Value = value;
        }
    }
}
