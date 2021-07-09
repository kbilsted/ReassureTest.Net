using System;
using System.Collections;
using System.Collections.Generic;

namespace ReassureTest.Net
{
    class TokenVisitor
    {
        private readonly HashSet<object> seenBefore = new HashSet<object>();

        public Dictionary<string, Action<IndentingStringBuilder, object>> SimpleTypeHandling =
            new Dictionary<string, Action<IndentingStringBuilder, object>>()
            {
                {typeof(int).ToString(), (sb, o) => sb.Add(o)},
                {typeof(bool).ToString(), (sb, o) => sb.Add(o)},
                {typeof(string).ToString(), (sb, o) => sb.Add($"\"{((string)o).Replace("\"","\\\"")}\"")},
                {typeof(long).ToString(), (sb, o) => sb.Add(o)},
                {typeof(short).ToString(), (sb, o) => sb.Add(o)},
                {typeof(Guid).ToString(), (sb, o) => sb.Add(o)},
                {typeof(DateTime).ToString(), (sb, o) => sb.Add(o)},
                {typeof(TimeSpan).ToString(), (sb, o) => sb.Add(o)},
            };


        public void Visit(List<Token> sb, object o)
        {
            if (o == null)
            {
                sb.Add(Token.Null);
                return;
            }

            Action<IndentingStringBuilder, object> code;

            if (SimpleTypeHandling.TryGetValue(o.GetType().ToString(), out code))
            {
                var x = new IndentingStringBuilder();
                code(x, o);
                sb.Add(new Token(){Type = "S",Value = x.ToString()});
                return;
            }

            // ienumerable
            if (o is IEnumerable enumerable)
            {
                var t = new Token()
                {
                    Type = "A"
                };
                sb.Add(t);
                foreach (var v in enumerable)
                {
                    if (SimpleTypeHandling.TryGetValue(v.GetType().ToString(), out code))
                    {
                        var x = new IndentingStringBuilder();
                        code(x, v);
                        sb.Add(new Token() { Type = "S", Value = x.ToString() });
                    }
                    else
                    {
                        Visit(sb, v);
                    }
                }
                sb.Add(Token.ArrayEnd);
                return;
            }

            if (seenBefore.Contains(o))
            {
                sb.Add(Token.Recursive);
                return;
            }
            seenBefore.Add(o);


            // object
            sb.Add(Token.ComplexStart);
            foreach (var propertyInfo in o.GetType().GetProperties())
            {
                sb.Add(new Token() { Type = "F", Value = propertyInfo.Name});
                Visit(sb, propertyInfo.GetValue(o));
            }
            sb.Add(Token.ComplexEnd);
        }
    }
}