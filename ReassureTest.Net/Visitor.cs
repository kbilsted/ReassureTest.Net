using System;
using System.Collections;
using System.Collections.Generic;

namespace ReassureTest.Net
{
    class Visitor
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


        public void Visit(IndentingStringBuilder sb, object o)
        {
            if (o == null)
            {
                sb.Add("null");
                return;
            }

            Action<IndentingStringBuilder, object> code;

            if (SimpleTypeHandling.TryGetValue(o.GetType().ToString(), out code))
            {
                code(sb, o);
                return;
            }

            // ienumerable
            if (o is IEnumerable enumerable)
            {
                sb.Add("[ ").Indent();
                bool first = true;
                foreach (var v in enumerable)
                {
                    if (first)
                        first = false;
                    else
                        sb.Add(",");

                    if (SimpleTypeHandling.TryGetValue(v.GetType().ToString(), out code))
                    {
                        sb.Add(" ");
                        code(sb, v);
                    }
                    else
                    {
                        sb.AddLine("").AddIndented("");
                        Visit(sb, v);
                    }
                }
                sb.Add(" ]").Dedent();
                return;
            }

            if (seenBefore.Contains(o))
            {
                sb.Add("[[Recursive Reference]]");
                return;
            }
            seenBefore.Add(o);


            // object
            sb.AddLine("{").Indent();
            foreach (var propertyInfo in o.GetType().GetProperties())
            {
                sb.AddIndented(propertyInfo.Name + " = ");
                Visit(sb, propertyInfo.GetValue(o));
                sb.AddLine("");
            }
            sb.Dedent().AddIndented("}");
        }
    }
}