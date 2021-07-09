using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ReassureTest.Net.AST;

namespace ReassureTest.Net
{
    public static class ReassureSetup
    {
        public static void Is_old(this object o, object actual)
        {
            var sb = new IndentingStringBuilder();
            new Visitor().Visit(sb, o);
            Console.WriteLine(sb);
        }

        public static void Is(this object o, object actual)
        {
            //var x = TokenVisitorParse(o, out var sb);

            //Console.WriteLine(string.Join("\n", x));
            //Console.WriteLine("*************");
            //Console.WriteLine("*************");
            //Console.WriteLine("*************");
            //Console.WriteLine(sb);


            Is_old(o, actual);

            Console.WriteLine("!!!!!!!!!!!!!");
            Console.WriteLine("!!!!!!!!!!!!!");
            Console.WriteLine("!!!!!!!!!!!!!");

            var ast = new AstVisitor().Visit(o, null);
            Console.WriteLine(new AstPrinter().PrintRoot(ast));
        }

        private static List<Token> TokenVisitorParse(object o, out IndentingStringBuilder sb)
        {
            var x = new List<Token>();
            new TokenVisitor().Visit(x, o);

            sb = new IndentingStringBuilder();
            bool isArray = false;
            int arrayCount;
            arrayCount = 0;
            for (int i = 0; i < x.Count; i++)
            {
                var token = x[i];

                if (token == Token.Null)
                    sb.Add("null");

                if (token == Token.ArrayStart)
                {
                    sb.Add("[").Indent();
                    arrayCount = 0;
                    isArray = true;
                }

                if (token == Token.ArrayEnd)
                {
                    sb.AddLine("]").Dedent();
                    isArray = false;
                }

                if (token == Token.ComplexStart)
                    sb.AddLine("{").Indent();
                if (token == Token.ComplexEnd)
                    sb.Dedent().AddLineIndented("}");
                if (token.Type == "F")
                    sb.AddIndented(token.Value + " = ");
                if (token.Type == "S")
                {
                    if (isArray)
                    {
                        if (arrayCount > 0)
                            sb.Add(", ");
                        arrayCount++;
                        sb.Add(token.Value);
                    }
                    else
                    {
                        sb.AddLine(token.Value);
                    }
                }
            }

            return x;
        }

        /// <summary>
        /// excpected, actual
        /// </summary>
        public static Action<object, object> Assert { get; set; }
    }

    class Token
    {
        public string Type { get; set; }
        public string Value { get; set; }
        public string[] Values { get; set; }

        public static Token Null = new Token() {Type = "null", Value = "null"};
        public static Token ArrayStart = new Token() {Type = "A", Value = "["};
        public static Token ArrayEnd= new Token() {Type = "A", Value = "]" };
        public static Token ComplexStart = new Token() { Type = "C", Value = "{" };
        public static Token ComplexEnd = new Token() { Type = "C", Value = "}" };
        public static Token Recursive = new Token() { Type = "R", Value = "SEEN BEFORE" };

        public override string ToString()
        {
            return Type + ": " +  Value + (Values!=null?string.Join(",",Values):"");
        }
    }

    class AstVisitor
    {
        private readonly HashSet<object> seenBefore = new HashSet<object>();

        public Dictionary</*typename*/string, Func<object, AstSimpleValue>> SimpleTypeHandling 
            = new Dictionary<string, Func<object, AstSimpleValue>>()
            {
                {typeof(int).ToString(), (o) => new AstSimpleValue(o)},
                {typeof(bool).ToString(), (o) => new AstSimpleValue(o)},
                {typeof(string).ToString(), (o) => new AstSimpleValue($"\"{((string)o).Replace("\"","\\\"")}\"")},
                {typeof(long).ToString(), (o) => new AstSimpleValue(o)},
                {typeof(short).ToString(), (o) => new AstSimpleValue(o)},
                {typeof(Guid).ToString(), (o) => new AstSimpleValue(o)},
                {typeof(DateTime).ToString(), (o) => new AstSimpleValue(o)},
                {typeof(TimeSpan).ToString(), (o) => new AstSimpleValue(o)},
            };

        public IValue Visit(object o, IValue parent)
        {
            if (o == null)
                return AstSimpleValue.Null;

            // simple
            Func<object, AstSimpleValue> code;
            if (SimpleTypeHandling.TryGetValue(o.GetType().ToString(), out code))
                return code(o);

            // re-discovered...
            if (seenBefore.Contains(o))
                return AstSimpleValue.SeenBefore;
            seenBefore.Add(o);

            // array
            if (o is IEnumerable enumerable)
            {
                var arr = new AstArray();
                foreach (var v in enumerable)
                    arr.Add(SimpleTypeHandling.TryGetValue(v.GetType().ToString(), out code) 
                        ? code(v) 
                        : Visit(v, arr));
                return arr;
            }

            // complex
            var c = new AstComplexValue();
            foreach (var propertyInfo in o.GetType().GetProperties())
                c.Values.Add(propertyInfo.Name, Visit(propertyInfo.GetValue(o), c));
            return c;
        }
    }
}
