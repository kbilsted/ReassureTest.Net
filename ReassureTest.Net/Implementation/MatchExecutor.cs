using System;
using System.Text.RegularExpressions;
using ReassureTest.AST;
using ReassureTest.AST.Expected;

namespace ReassureTest.Implementation
{
    public class MatchExecutor
    {
        private readonly Configuration configuration;

        public MatchExecutor(Configuration configuration)
        {
            this.configuration = configuration;
        }

        public void MatchGraph(IAssertEvaluator expected, IAstNode actual) => Match(expected, actual, "");

        string AddPath(string path, string newLevel)
        {
            if (path == "")
                return newLevel;
            if (newLevel[0] == '[')
                return path + newLevel;
            return $"{path}.{newLevel}";
        }

        void Match(IAssertEvaluator expected, IAstNode actual, string path)
        {
            if (expected is AstDateTimeMatcher dateTime)
                DateTimeMatch(dateTime, actual, path);
            else if (expected is AstStringMatcher str)
                StringMatch(str, actual, path);
            else if (expected is AstGuidMatcher guid)
                GuidMatch(guid, actual, path);
            else if (expected is AstSimpleMatcher simple)
                SimpleMatch(simple, actual, path);
            else if (expected is AstComplexMatcher complex)
                ComplexMatch(complex, actual, path);
            else if (expected is AstArrayMatcher array)
                ArrayMatch(array, actual, path);
            else if (expected is AstAnyMatcher)
                AnyMatch();
            else if (expected is AstSomeMatcher)
                SomeMatch(actual, path);
            else
                throw new AssertException($"Internal error. Do not understand '{expected.GetType()}' to be compared with '{actual}'. {PrintablePath(path)}");
        }



        private void SomeMatch(IAstNode actual, string path)
        {
            if (actual == AstSimpleValue.Null)
                throw new AssertException($"{PrintablePath(path)}Expected: not null\r\nBut was: null");
        }

        private void AnyMatch()
        {
            // always true
        }

        void ArrayMatch(AstArrayMatcher array, IAstNode actual, string path)
        {
            SomeMatch(actual, path);

            if (actual is AstArray arrayActual)
            {
                string newPath;

                for (int i = 0; i < array.Value.Values.Count; i++)
                {
                    newPath = AddPath(path, $"[{i}]");
                    if (arrayActual.Values.Count - 1 < i)
                        throw new AssertException($"{PrintablePath(newPath)}Array length mismatch. Expected array lengh: {array.Value.Values.Count} but was: {arrayActual.Values.Count}.");
                    var theActual = arrayActual.Values[i];

                    var theExpected = (IAssertEvaluator)array.Value.Values[i];
                    Match(theExpected, theActual, newPath);
                }

                newPath = AddPath(path, $"[{array.Value.Values.Count + 1}]");
                if (arrayActual.Values.Count > array.Value.Values.Count)
                    throw new AssertException($"{PrintablePath(newPath)}Array length mismatch. Expected array lengh: {array.Value.Values.Count} but was: {arrayActual.Values.Count}.");
            }
            else
            {
                throw new AssertException($"Wrong type of actual value. Expected array got {actual.GetType()}. {PrintablePath(path)}");
            }
        }

        void ComplexMatch(AstComplexMatcher complex, IAstNode actual, string path)
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
                        throw new AssertException($"{PrintablePath(path)}Cannot find field '{kv.Key}' in expected values.");
                    }
                }
            }
            else
            {
                throw new AssertException($"Wrong type. Expected complex value got {actual.GetType()}. {PrintablePath(path)}");
            }
        }

        private void DateTimeMatch(AstDateTimeMatcher dateTimeMatcher, IAstNode actual, string path)
        {
            SomeMatch(actual, path);

            if (actual is AstSimpleValue simpleActual)
            {
                if (!(dateTimeMatcher.UnderlyingValue.Value is DateTime expectedDate))
                    throw new AssertException($"{PrintablePath(path)}Expected: {dateTimeMatcher.UnderlyingValue.Value}\r\nBut was:  {simpleActual.Value}");

                if (!(simpleActual.Value is DateTime actualDate))
                    throw new AssertException($"{PrintablePath(path)}Expected: {dateTimeMatcher.UnderlyingValue.Value}\r\nBut was:  {simpleActual.Value}");

                if (!IsAlmostSame(expectedDate, actualDate, dateTimeMatcher.AcceptedSlack))
                    Compare(expectedDate, actualDate, path, configuration);
            }
            else
            {
                throw new AssertException($"Wrong type. Expected simple value got {actual.GetType()}. {PrintablePath(path)}");
            }
        }

        private void StringMatch(AstStringMatcher str, IAstNode actual, string path)
        {
            if (actual is AstSimpleValue simpleActual)
            {
                if (simpleActual.Value == null)
                    throw new AssertException($"{PrintablePath(path)}Expected: \"{str.UnderlyingValue.Value}\"\r\nBut was:  null");

                if (!(simpleActual.Value is string actualString))
                    throw new AssertException($"{PrintablePath(path)}Expected: \"{str.UnderlyingValue.Value}\"\r\nBut was:  \"{simpleActual.Value}\"");

                if (!(str.UnderlyingValue.Value is string expectedString))
                    throw new AssertException($"{PrintablePath(path)}Expected: \"{str.UnderlyingValue.Value}\"\r\nBut was:  \"{simpleActual.Value}\"");

                if (!Regex.IsMatch(actualString, expectedString.Replace("*", ".*"), RegexOptions.Singleline))
                    throw new AssertException($@"{PrintablePath(path)}Expected: ""{expectedString}""
But was:  ""{actualString}""");
            }
            else
            {
                throw new AssertException($"Wrong type. Expected simple value got {actual.GetType()}. Path: '{path}'");
            }
        }

        private void GuidMatch(AstGuidMatcher guidMatcher, IAstNode actual, string path)
        {
            if (actual is AstSimpleValue simpleActual)
            {
                if (!(simpleActual.Value is AstRollingGuid actualRg))
                    throw new AssertException($"{PrintablePath(path)}Expected: {guidMatcher.UnderlyingValue}\r\nBut was:  {simpleActual.Value}");

                Compare(guidMatcher.UnderlyingValue, actualRg, path, configuration);
            }
            else
            {
                throw new AssertException($"Wrong type. Expected simple value got {actual.GetType()}. Path: '{path}'");
            }
        }

        void SimpleMatch(AstSimpleMatcher simple, IAstNode actual, string path)
        {
            if (actual is AstSimpleValue simpleActual)
            {
                Compare(simple.UnderlyingValue.Value, simpleActual.Value, path, configuration);
            }
            else
            {
                throw new AssertException($"Wrong type. Expected simple value got {actual.GetType()}. Path: '{path}'");
            }
        }

        public static string PrintablePath(string path) => path == "" ? "" : $"Path: '{path}'.\r\n";

        public static void Compare(object expected, object actual, string path, Configuration cfg)
        {
            if (expected == null)
            {
                if (actual == null)
                    return;

                string actualValue = actual is string ss ? $"\"{ss}\"" : actual.ToString();
                throw new AssertException($@"{PrintablePath(path)}Expected: null
But was:  {actualValue}");
            }

            if (actual == null)
            {
                string expectedValue = expected is string ss ? $"\"{ss}\"" : expected.ToString();
                throw new AssertException($@"{PrintablePath(path)}Expected: {expectedValue}
But was:  null");

            }


            string theExpedted = expected is DateTime de
                ? de.ToString(cfg.Assertion.DateTimeFormat)
                : expected.ToString();
            string theActual = actual is DateTime da ? da.ToString(cfg.Assertion.DateTimeFormat) : actual.ToString();
            if (!theExpedted.Equals(theActual))
                throw new AssertException($@"{PrintablePath(path)}Expected: {theExpedted}
But was:  {theActual}");
        }

        public static bool IsAlmostNow(DateTime d1, TimeSpan slack) => IsAlmostSame(d1, DateTime.Now, slack);

        public static bool IsAlmostSame(DateTime d1, DateTime d2, TimeSpan slack)
        {
            return (d1 - d2).Duration() <= slack;
        }
    }
}