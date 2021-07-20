using System;
using ReassureTest.AST;
using ReassureTest.AST.Expected;

namespace ReassureTest.Implementation
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
            if (expected is AstDateTimeMatcher dateTime)
                DateTimeMatch(dateTime, actual, path);
            else if (expected is AstSimpleMatcher simple)
                SimpleMatch(simple, actual, path);
            else if (expected is AstComplexMatcher complex)
                ComplexMatch(complex, actual, path);
            else if (expected is AstArrayMatcher array)
                ArrayMatch(array, actual, path);
            else if (expected is AstAnyMatcher any)
                AnyMatch(any, actual, path);
            else if (expected is AstSomeMatcher)
                SomeMatch(actual, path);
            else
                throw new AssertException($"Internal error. Do not understand '{expected.GetType()}' to be compared with '{actual}'. Path: '{path}'");
        }

        private void SomeMatch(IValue actual, string path)
        {
            if (actual == AstSimpleValue.Null)
                throw new AssertException($"Path: '{path}'. Expected: not null\r\nBut was: null");
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

        private void DateTimeMatch(AstDateTimeMatcher dateTimeMatcher, IValue actual, string path)
        {
            SomeMatch(actual, path);

            if (actual is AstSimpleValue simpleActual)
            {
                if (!(dateTimeMatcher.UnderlyingValue.Value is DateTime expectedDate))
                    throw new AssertException($"Path: '{path}'. Expected {dateTimeMatcher.UnderlyingValue.Value}, but was {simpleActual.Value}");

                if (!(simpleActual.Value is DateTime actualDate))
                    throw new AssertException($"Path: '{path}'. Expected {dateTimeMatcher.UnderlyingValue.Value}, but was {simpleActual.Value}");

                if ((expectedDate - actualDate).Duration() > dateTimeMatcher.AcceptedSlack)
                    CallUnitTestingFramework(dateTimeMatcher.UnderlyingValue.Value, simpleActual.Value, path);
            }
            else
            {
                throw new AssertException($"Wrong type. Expected simple value got {actual.GetType()}. Path: '{path}'");
            }
        }

        void SimpleMatch(AstSimpleMatcher simple, IValue actual, string path)
        {
            if (actual is AstSimpleValue simpleActual)
            {
                CallUnitTestingFramework(simple.UnderlyingValue.Value, simpleActual.Value, path);
            }
            else
            {
                throw new AssertException($"Wrong type. Expected simple value got {actual.GetType()}. Path: '{path}'");
            }
        }

        /// <summary>
        /// calling the underlying assert has advantages
        /// * errors looks like the rest of the test output / is familiar
        /// * errors respect locale, e.g. formatting dates printed when mis-matching
        /// * when errors, frameworks tend to print a nice arrow pointing to the difference
        /// </summary>
        void CallUnitTestingFramework(object expected, object actual, string path)
        {
            try
            {
                assert(expected, actual);
            }
            catch (Exception e)
            {
                throw new AssertException($"Path: '{path}'. {e.Message.TrimStart()}");
            }
        }
    }
}