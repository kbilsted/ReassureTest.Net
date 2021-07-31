using System;
using System.Collections.Generic;
using ReassureTest.DSL;

namespace ReassureTest.AST
{
    public interface IValue { }
    
    enum ValueKind
    {
        Simple, Complex
    }

    public class AstSimpleValue : IValue
    {
        public static readonly AstSimpleValue Null = new AstSimpleValue(null);
        public static readonly AstSimpleValue SeenBefore = new AstSimpleValue("[[SEEN BEFORE]]");

        public object Value;

        public AstSimpleValue(object o)
        {
            if (o is DslToken || o is DslToken[])
                throw new ArgumentException($"Internal error: Illegal input data of type {o.GetType()}");

            Value = o;
        }
    }

    public class AstComplexValue : IValue
    {
        public Dictionary</*fieldname*/string, IValue> Values { get; set; } = new Dictionary<string, IValue>();
    }

    class AstArray : IValue
    {
        public ValueKind ArrayKind { get; set; } = ValueKind.Simple;
        public List<IValue> Values { get; set; } = new List<IValue>();

        public void Add(IValue v)
        {
            if (v is AstComplexValue || v is AstArray)
                ArrayKind = ValueKind.Complex;

            Values.Add(v);
        }
    }

}