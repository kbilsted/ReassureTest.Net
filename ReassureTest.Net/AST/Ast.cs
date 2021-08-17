using System;
using System.Collections.Generic;
using ReassureTest.DSL;

namespace ReassureTest.AST
{
    public interface IAstNode { }
    
    enum ValueKind
    {
        Simple, Complex
    }

    public class AstSimpleValue : IAstNode
    {
        public static readonly AstSimpleValue Null = new AstSimpleValue(null);
        public static readonly AstSimpleValue SeenBefore = new AstSimpleValue("[[SEEN BEFORE]]");

        public object? Value;

        public AstSimpleValue(object? o)
        {
            if (o is DslToken || o is DslToken[])
                throw new ArgumentException($"Internal error: Illegal input data of type {o.GetType()}");

            Value = o;
        }
    }

    public class AstComplexValue : IAstNode
    {
        public Dictionary</*fieldname*/string, IAstNode> Values { get; set; } = new Dictionary<string, IAstNode>();
    }

    class AstArray : IAstNode
    {
        public ValueKind ArrayKind { get; set; } = ValueKind.Simple;
        public List<IAstNode> Values { get; set; } = new List<IAstNode>();

        public void Add(IAstNode v)
        {
            if (v is AstComplexValue || v is AstArray)
                ArrayKind = ValueKind.Complex;

            Values.Add(v);
        }
    }
}