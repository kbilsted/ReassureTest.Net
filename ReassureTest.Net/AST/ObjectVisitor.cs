using System;
using System.Collections;
using System.Collections.Generic;

namespace ReassureTest.AST
{
    public class ObjectVisitor
    {
        private readonly Configuration configuration;
        private readonly HashSet<object> seenBefore = new HashSet<object>();

        static Dictionary</*typename*/string, Func<object, AstSimpleValue>> SimpleTypeHandling
           = new Dictionary<string, Func<object, AstSimpleValue>>()
           {
                {typeof(int).ToString(), o => new AstSimpleValue(o)},
                {typeof(bool).ToString(), o => new AstSimpleValue(o)},
                {typeof(string).ToString(), o => new AstSimpleValue(o)},
                {typeof(long).ToString(), o => new AstSimpleValue(o)},
                {typeof(float).ToString(), o => new AstSimpleValue(o)},
                {typeof(double).ToString(), o => new AstSimpleValue(o)},
                {typeof(decimal).ToString(), o => new AstSimpleValue(o)},
                {typeof(short).ToString(), o => new AstSimpleValue(o)},
                {typeof(Guid).ToString(), o => new AstSimpleValue(o)},
                {typeof(DateTime).ToString(), o => new AstSimpleValue(o)},
                {typeof(TimeSpan).ToString(), o => new AstSimpleValue(o)},
           };


        public ObjectVisitor(Configuration configuration)
        {
            this.configuration = configuration;
        }

        public IValue Visit(object o)
        {
            // map
            foreach (Func<object, object> translator in configuration.Harvesting.FieldValueTranslators)
            {
                if (o == null)
                    break;
                o = translator(o);
            }


            // null
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
                    arr.Add(Visit(v));
                return arr;
            }

            // complex
            var c = new AstComplexValue();

            if (configuration.Outputting.EnableDebugPrint)
                configuration.Outputting.Print($"ObjectVisitor: Investigating '{o.GetType()}'");

            foreach (var propertyInfo in o.GetType().GetProperties())
                c.Values.Add(propertyInfo.Name, Visit(propertyInfo.GetValue(o)));
            return c;
        }
    }
}