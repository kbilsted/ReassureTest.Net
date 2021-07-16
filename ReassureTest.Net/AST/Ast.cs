using System;
using System.Collections.Generic;
using ReassureTest.Net.DSL;

namespace ReassureTest.Net.AST
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

        public AstSimpleValue(object o)
        {
            if (o is DslToken || o is DslToken[])
                throw new ArgumentException($"Internal error: Illegal input data of type {o.GetType()}");

            Value = o;
        }

        public object Value;
    }

    public class AstComplexValue : IValue
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

    public interface IAssertEvaluator : IValue
    {
    }

    /// <summary>
    /// a '?' matcher
    /// </summary>
    class AstAnyMatcher : IAssertEvaluator
    {
    }

    /// <summary>
    /// A '*' matcher
    /// </summary>
    class AstSomeMatcher : IAssertEvaluator
    {
    }

    /// <summary>
    /// An exact matcher, eg. i=3
    /// </summary>
    class AstSimpleMatcher : IAssertEvaluator
    {
        public AstSimpleValue UnderlyingValue { get; set; }
     
        public AstSimpleMatcher(AstSimpleValue value)
        {
            UnderlyingValue = value;
        }
    }

    class AstDateTimeMatcher : IAssertEvaluator
    {
        public AstSimpleValue UnderlyingValue { get; set; }
        public readonly TimeSpan AcceptedSlack;

        public AstDateTimeMatcher(AstSimpleValue value, TimeSpan acceptedSlack)
        {
            UnderlyingValue = value;
            AcceptedSlack = acceptedSlack;
        }
    }

    /// <summary>
    /// An eg. {a=2,b=3} = {a=2,b=3}
    /// </summary>
    class AstComplexMatcher : IAssertEvaluator
    {
        public AstComplexMatcher(AstComplexValue value)
        {
            Value = value;
        }

        public AstComplexValue Value { get; set; }
    }

    class AstArrayMatcher : IAssertEvaluator
    {
        public AstArrayMatcher(AstArray value)
        {
            Value = value;
        }

        public AstArray Value { get; set; }
    }
}