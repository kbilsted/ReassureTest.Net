﻿using System;
using System.Collections;
using System.Reflection;
using ReassureTest.Implementation;

namespace ReassureTest
{
    public static class ReusableProjections
    {
        public static Flow SimplifyExceptions(object parent, object field, PropertyInfo pi)
        {
            if (parent is SimplifiedException 
                && pi.Name == "Data" 
                && field is IDictionary d 
                && d.Keys.Count == 0)
            {
                  return Flow.Skip;
            }
            return Flow.Use(field);
        }

        public static Flow FixDefaultImmutableArrayCanNotBeTraversed(object parent, object field, PropertyInfo pi)
        {
            var type = field.GetType().ToString();
            if (!type.StartsWith("System.Collections.Immutable.ImmutableArray", StringComparison.Ordinal))
                return Flow.Use(field);

            try
            {
                ((IEnumerable)field).GetEnumerator();
            }
            catch (InvalidOperationException)
            {
                return Flow.UseNull;
            }

            return Flow.Use(field);
        }

        public static Flow SkipUnharvestableTypes(object parent, object field, PropertyInfo pi)
        {
            var typename = field.GetType().ToString();
            if (typename.StartsWith("System.Reflection", StringComparison.Ordinal)
                || typename.StartsWith("System.Runtime", StringComparison.Ordinal)
                || typename.StartsWith("System.SignatureStruct", StringComparison.Ordinal)
                || typename.StartsWith("System.Func", StringComparison.Ordinal))
                return Flow.Skip;
            return Flow.Use(field);
        }
    }
}