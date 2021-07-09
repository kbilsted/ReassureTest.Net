using System.Collections.Generic;

namespace ReassureTest.Net.AST
{
    interface IValue { }
    
    enum ValueKind
    {
        Simple, Complex
    }

    internal class AstSimpleValue : IValue
    {
        public static readonly AstSimpleValue Null = new AstSimpleValue("null" );
        public static readonly AstSimpleValue SeenBefore = new AstSimpleValue("[[SEEN BEFORE]]");

        public AstSimpleValue(object o)
        {
            Value = o.ToString();
        }

        public object Value;
    }

    internal class AstComplexValue : IValue
    {
        public Dictionary</*fieldname*/string, IValue> Values = new Dictionary<string, IValue>();
    }

    class AstArray : IValue
    {
        public ValueKind ArrayKind = ValueKind.Simple;
        public List<IValue> Values = new List<IValue>();

        public void Add(IValue v)
        {
            if (v is AstComplexValue || v is AstArray)
                ArrayKind = ValueKind.Complex;

            Values.Add(v);
        }
    }
}