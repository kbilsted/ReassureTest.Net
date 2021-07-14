using System;
using ReassureTest.Net.AST;

namespace ReassureTest.Net
{
    public class MatchExecutor
    {
        private readonly Action<object, object> assert;

        public MatchExecutor(Action<object, object> assert)
        {
            this.assert = assert;
        }

        public void MatchGraph(IAssertEvaluator expected, IValue actual) => Match(expected, actual, "");

        string AddPath(string path, string newLevel)
        {
            if (path == "")
                return newLevel;
            if (newLevel[0] == '[')
                return path + newLevel;
            return $"{path}.{newLevel}";
        }

        void Match(IAssertEvaluator expected, IValue actual, string path)
        {
            if (expected is AstSimpleMatcher simple)
                SimpleMatch(simple, actual, path);
            else if (expected is AstComplexMatcher complex)
                ComplexMatch(complex, actual, path);
            else if (expected is AstArrayMatcher array)
                ArrayMatch(array, actual, path);
            else if (expected is AstAnyMatcher any)
                AnyMatch(any, actual, path);
            else if (expected is AstSomeMatcher some)
                SomeMatch(actual, path);
            else
                throw new AssertException($"Internal error. Do not understand '{expected.GetType()}' to be compared with '{actual}'. Path: '{path}'");
        }

        private void SomeMatch(IValue actual, string path)
        {
            if (actual == AstSimpleValue.Null)
                throw new AssertException($"Path: '{path}'. Expected: not null\nBut was: null");
        }

        private void AnyMatch(AstAnyMatcher anyMatcher, IValue actual, string path)
        {
            // always true
        }

        void ArrayMatch(AstArrayMatcher array, IValue actual, string path)
        {
            SomeMatch(actual, path);

            if (actual is AstArray arrayActual)
            {
                string newPath;

                for (int i = 0; i < array.Value.Values.Count; i++)
                {
                    newPath = AddPath(path, $"[{i}]");
                    if (arrayActual.Values.Count - 1 < i)
                        throw new AssertException($"Path: '{newPath}'. Array length mismatch. Expected array lengh: {array.Value.Values.Count} but was: {arrayActual.Values.Count}.");
                    var theActual = arrayActual.Values[i];

                    var theExpected = (IAssertEvaluator)array.Value.Values[i];
                    Match(theExpected, theActual, newPath);
                }

                newPath = AddPath(path, $"[{array.Value.Values.Count + 1}]");
                if (arrayActual.Values.Count > array.Value.Values.Count)
                    throw new AssertException($"Path: '{newPath}'. Array length mismatch. Expected array lengh: {array.Value.Values.Count} but was: {arrayActual.Values.Count}.");
            }
            else
            {
                throw new AssertException($"Wrong type of actual value. Expected array got {actual.GetType()}. Path: '{path}'");
            }
        }

        void ComplexMatch(AstComplexMatcher complex, IValue actual, string path)
        {
            SomeMatch(actual, path);

            if (actual is AstComplexValue complexActual)
            {
                foreach (var kv in complexActual.Values)
                {
                    if (complex.Value.Values.TryGetValue(kv.Key, out var matcher))
                    {
                        Match(matcher as IAssertEvaluator, kv.Value, AddPath(path, kv.Key));
                    }
                    else
                    {
                        throw new AssertException($"Path: '{path}'. Cannot find field '{kv.Key}' in expected values.");
                    }
                }
            }
            else
            {
                throw new AssertException($"Wrong type. Expected complex value got {actual.GetType()}. Path: '{path}'");
            }
        }

        void SimpleMatch(AstSimpleMatcher simple, IValue actual, string path)
        {
            if (actual is AstSimpleValue simpleActual)
            {
                try
                {
                    assert(simple.UnderlyingValue.Value, simpleActual.Value);
                }
                catch (Exception e)
                {
                    throw new AssertException($"Path: '{path}'. {e.Message.TrimStart()}");
                }
            }
            else
            {
                throw new AssertException($"Wrong type. Expected simple value got {actual.GetType()}. Path: '{path}'");
            }
        }
    }
}