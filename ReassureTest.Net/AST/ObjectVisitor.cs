using System;
using System.Collections;
using System.Collections.Generic;

namespace ReassureTest.AST
{
    public class ObjectVisitor
    {
        private readonly Configuration configuration;
        private readonly HashSet<object> seenBefore = new HashSet<object>();
        private int rollingGuidCounter = 0;
        private readonly Dictionary<Guid, int> rollingGuidValues = new Dictionary<Guid, int>();

        bool SimpleTypeHandling(object o, out AstSimpleValue result)
        {
            if (o is int
                || o is bool
                || o is string
                || o is long
                || o is float
                || o is double
                || o is decimal
                || o is short
                || o is DateTime
                || o is TimeSpan)
            {
                result = new AstSimpleValue(o);
                return true;
            }

            if (o is Guid g)
            {
                switch (configuration.Assertion.GuidHandling)
                {
                    case Configuration.GuidHandling.Exact:
                        result = new AstSimpleValue(o);
                        return true;

                    case Configuration.GuidHandling.Rolling:
                        if (!rollingGuidValues.TryGetValue(g, out int count))
                        {
                            count = rollingGuidCounter++;
                            rollingGuidValues.Add(g, count);
                        }

                        result = new AstSimpleValue(new AstRollingGuid(count));
                        return true;

                    default:
                        throw new ArgumentOutOfRangeException(configuration.Assertion.GuidHandling.ToString());
                }
            }

            result = null;
            return false;
        }

        public ObjectVisitor(Configuration configuration)
        {
            this.configuration = configuration;
        }

        public IValue Visit(object o)
        {
            // translate
            foreach (Func<object, object> translator in configuration.Harvesting.FieldValueTranslators)
            {
                if (o == null)
                    return AstSimpleValue.Null;
                o = translator(o);
            }

            // null
            if (o == null)
                return AstSimpleValue.Null;

            // simple
            if (SimpleTypeHandling(o, out var result))
                return result;
            
            // re-discovered...
            if (seenBefore.Contains(o))
                return AstSimpleValue.SeenBefore;
            seenBefore.Add(o);

            // array
            if (o is IEnumerable enumerable)
            {
                var arr = new AstArray();
                foreach (var v in enumerable)
                    arr.Add(Visit(v));
                return arr;
            }

            // complex
            var c = new AstComplexValue();

            if (configuration.Outputting.EnableDebugPrint)
                configuration.TestFrameworkIntegration.Print($"ObjectVisitor: Investigating '{o.GetType()}'");

            foreach (var propertyInfo in o.GetType().GetProperties())
                c.Values.Add(propertyInfo.Name, Visit(propertyInfo.GetValue(o)));
            return c;
        }
    }
}