using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReassureTest.Net.AST;

namespace ReassureTest.Net
{
    public static class ReassureSetup
    {
        public static void Is(this object actual, string expected)
        {
            new ReassureTestTester().Is(actual, expected, Print, Assert);
        }

        /// <summary>
        /// excpected, actual
        /// </summary>
        public static Action</*expected*/object, /*actual*/object> Assert { get; set; }

        public static Action<string> Print { get; set; } = Console.WriteLine;
    }

    /// <summary>
    /// Use this when testing ReassureTest.Net
    /// </summary>
    public class ReassureTestTester
    {
        public void Is(object actual, string expected, Action<string> print, Action</*expected*/object, /*actual*/object> assert)
        {
            IValue astActual = new ObjectVisitor().Visit(actual);

            IValue expectedAst = new DslParser(new DslTokenizer(Console.WriteLine
                                                             )).Parse(expected);

            if (expectedAst == null)
            {
                string graph = new AstPrinter().PrintRoot(astActual);
                print($"Actual is:\n{graph}");

                assert(graph, expected);
                return;
            }

            var executor = new MatchExecutor(assert);
            try
            {
                executor.MatchGraph(expectedAst as IAssertEvaluator, astActual);
            }
            catch (Exception)
            {
                string graph = new AstPrinter().PrintRoot(astActual);
                print($"Actual is:\n{graph}");
                throw;
            }
        }

        public void Is(object actual, string expected, Action</*expected*/object, /*actual*/object> assert) => Is(actual, expected, Console.WriteLine, assert);
    }


    public class DslParser
    {
        private readonly DslTokenizer tokenizer;
        private int i = 0;
        DslToken[] tokens;

        public DslParser(DslTokenizer tokenizer)
        {
            this.tokenizer = tokenizer;
        }

        public IValue Parse(string s)
        {
            tokens = tokenizer.Tokenize(s).ToArray();
            if (tokens.Length == 0)
                return null;
            return ParseIValue();
        }

        bool PeekValueOrString()
        {
            DslToken t = tokens[i];
            return t.Kind == DslTokenizer.TokenKind.String || t.Kind == DslTokenizer.TokenKind.Value;
        }

        void PeekEatMeta(string s)
        {
            if (PeekMeta(s))
                EatMeta(s);
        }

        bool PeekMeta(string s)
        {
            DslToken t = tokens[i];
            return t.Kind == DslTokenizer.TokenKind.Meta && t.Value.ToString() == s;
        }

        void EatMeta(string s)
        {
            if (!PeekMeta(s))
            {
                DslToken t = tokens[i];
                throw new Exception($"Expected '{s}' got '{t.Value}' of kind '{t.Kind}' at token: {i}");
            }
            i++;
        }

        object EatValue()
        {
            DslToken t = tokens[i];
            if (t.Kind != DslTokenizer.TokenKind.Value)
                throw new Exception($"Expected a word got '{t.Value}' of kind '{t.Kind}' at token: {i}");
            i++;
            return t.Value;
        }

        object EatValueOrString()
        {
            DslToken t = tokens[i];
            if (t.Kind != DslTokenizer.TokenKind.Value && t.Kind != DslTokenizer.TokenKind.String)
                throw new Exception($"Expected a word or string got '{t.Value}' of kind '{t.Kind}' at token: {i}");
            i++;
            return t.Value;
        }

        public IAssertEvaluator ParseIValue()
        {
            if (PeekMeta("{"))
                return ParseComplex();
            if (PeekMeta("["))
                return ParseArray();
            if (PeekValueOrString())
                return ParseSimple();

            DslToken t = tokens[i];
            throw new Exception($"Unparseable '{t.Value}' of kind '{t.Kind}' at token: '{i}'");
        }

        private IAssertEvaluator ParseSimple()
        {
            var token = EatValueOrString();
            if(token is string str)
            {
                switch (str)
                {
                    case "?":
                        return new AstAnyMatcher();
                    case "*":
                        return new AstSomeMatcher();
                }
            }

            return new AstSimpleMatcher(new AstSimpleValue(token));
        }

        private IAssertEvaluator ParseArray()
        {
            EatMeta("[");
            var array = new AstArray();
            while (!PeekMeta("]"))
            {
                var value = ParseIValue();
                array.Add(value);
                PeekEatMeta(",");
            }
            EatMeta("]");
            return new AstArrayMatcher(array);
        }

        private IAssertEvaluator ParseComplex()
        {
            EatMeta("{");
            var c = new AstComplexValue();
            while (!PeekMeta("}"))
            {
                var name = (string)EatValue();
                EatMeta("=");
                var value = ParseIValue();
                c.Values.Add(name, value);
            }
            EatMeta("}");
            return new AstComplexMatcher(c);
        }
    }

    public class DslTokenizer
    {
        private readonly Action<string> print;

        public DslTokenizer(Action<string> print = null)
        {
            this.print = print;
        }

        public void WriteLine(string s)
        {
            if (print != null)
                print(s);
        }

        public enum TokenKind
        {
            String, Value, Meta
        }

        bool IsMeta(string s, int i) => s[i] == '=' || s[i] == '[' || s[i] == ']' || s[i] == '{' || s[i] == '}' || s[i] == ',';

        bool IsQuote(string s, int i)
        {
            if (s[i] != '"')
                return false;

            WriteLine($"IsQuote @{i} '{s[i]}'");

            int j = i - 1, backslashCount = 0;
            while (j > 0 && s[j] == '\\')
            {
                j--;
                backslashCount++;
            }

            var result = backslashCount % 2 == 0;
            WriteLine($"backslashCount {backslashCount} result {result}");

            return result;
        }

        bool IsSeparator(string s, int i) => char.IsWhiteSpace(s[i]) || IsMeta(s, i) || IsQuote(s, i);

        public List<DslToken> Tokenize(string s)
        {
            if (s == null)
                throw new ArgumentNullException(s);

            WriteLine($"Tokenizing: {s}");

            var tokens = new List<DslToken>();

            if (string.IsNullOrWhiteSpace(s))
                return tokens;

            int pos = 0;
            while (pos < s.Length)
            {
                if (char.IsWhiteSpace(s[pos]))
                {
                    pos++;
                    continue;
                }

                if (IsMeta(s, pos))
                {
                    Add(new DslToken(TokenKind.Meta, s[pos].ToString(), pos));
                    pos++;
                    continue;
                }

                int start = pos;

                if (IsQuote(s, pos))
                {
                    do
                    {
                        pos++;
                        if (pos >= s.Length)
                        {
                            var preview = PreviewString();
                            throw new Exception($"Unmatched quote starting at pos: {start}\n...{preview}...\n               ^");
                        }
                    } while (!IsQuote(s, pos));

                    var substring = s.Substring(start + 1, pos - start - 1);
                    substring = substring.Replace("\\", "\\\\").Replace("\\\"", "\"");
                    Add(new DslToken(TokenKind.String, substring, start));
                    pos++;
                    continue;
                }

                do
                {
                    pos++;
                } while (pos < s.Length && !IsSeparator(s, pos));
                Add(new DslToken(TokenKind.Value, s.Substring(start, pos - start), start));
                continue;

                string PreviewString()
                {
                    return s[new Range(Math.Max(0, start - 22), Math.Min(start + 22, s.Length))]
                        .Replace('\n', ' ')
                        .Replace('\r', ' ')
                        .Replace('\t', ' ');
                }
            }

            return tokens;

            void Add(DslToken t)
            {
                WriteLine($"New token @{pos} {t.Value}");
                tokens.Add(t);
            }
        }
    }

    public class DslToken
    {
        public readonly DslTokenizer.TokenKind Kind;
        public readonly object Value;
        public readonly int Pos;

        public DslToken(DslTokenizer.TokenKind kind, string value, int pos)
        {
            Kind = kind;
            Value = value;
            Pos = pos;

            if (value == "null")
                Value = null;
            else if (bool.TryParse(value, out var b))
                Value = b;
            else if (long.TryParse(value, out var llong))
                Value = llong;
            else if (decimal.TryParse(value, out var dec))
                Value = dec;
            else if (Guid.TryParse(value, out var guid))
                Value = guid;
        }

        public override string ToString() => $"{{{Kind}:{Value} (@ {Pos})}}";
    }
}
