using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frontend
{
    public class SymbolTable
    {
        private Dictionary<string, Symbol> SymbolDic = new Dictionary<string, Symbol>();

        public bool AddSymbol(string name, ExprType type, dynamic value = null)
        {
            if (SymbolDic.ContainsKey(name))
            {
                return false;
            }
            SymbolDic.Add(name, new Symbol(name, type, value));
            return true;
        }

        public ExprType? GetSymbolType(string name)
        {
            if (SymbolDic.ContainsKey(name))
                return SymbolDic[name].Type;
            return null;
        }

        public Symbol GetSymbol(string name)
        {
            if (SymbolDic.ContainsKey(name))
                return SymbolDic[name];
            return null;
        }

        public bool SetSymbolValue(string name, object value)
        {
            if (SymbolDic.ContainsKey(name))
            {
                SymbolDic[name].Value = value;
                return true;
            }
            return false;   
        }

        public object GetSymbolValue(string name)
        {
            if (SymbolDic.ContainsKey(name))
                return SymbolDic[name].Value;
            return null;
        }
    }

    public class Symbol
    {
        public readonly string Name;
        public readonly ExprType Type;
        public object Value { get; set; }

        public Symbol(string name, ExprType type, object value = null)
        {
            Name = name;
            Type = type;
            Value = value;
        }
    }
}
