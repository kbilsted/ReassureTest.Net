using System;
using System.Collections;

namespace ReassureTest
{
    /// <summary>
    /// Reusable implementations for field value translation
    /// </summary>
    public static class FieldValueTranslatorImplementations
    {
        public static object SimplifyExceptions(object o)
        {
            if (o is Exception ex)
                return new SimplifiedException(ex);
            return o;
        }

        public static object IgnoreUnharvestableTypes(object o)
        {
            var typename = o.GetType().ToString();
            if (typename.StartsWith("System.Reflection", StringComparison.Ordinal)
                || typename.StartsWith("System.Runtime", StringComparison.Ordinal)
                || typename.StartsWith("System.SignatureStruct", StringComparison.Ordinal)
                || typename.StartsWith("System.Func", StringComparison.Ordinal))
                return null;
            return o;
        }
    }

    public class SimplifiedException
    {
        public string Message { get; set; }
        public IDictionary Data { get; set; }
        public string Type { get; set; }

        public SimplifiedException(Exception e)
        {
            Message = e.Message;
            Data = e.Data;
            Type = e.GetType().ToString();
        }
    }

}