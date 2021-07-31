using System;
using System.Collections.Generic;

namespace ReassureTest.DSL
{
    public class DslTokenizer
    {
        private readonly Configuration configuration;

        public DslTokenizer(Configuration configuration)
        {
            this.configuration = configuration;
        }

        public void WriteLine(string s)
        {
            if (configuration.Outputting.EnableDebugPrint)
                configuration.TestFrameworkIntegration.Print(s);
        }

        public enum TokenKind
        {
            String, Value, Meta
        }

        bool IsMeta(string s, int i) => s[i] == '=' || s[i] == '[' || s[i] == ']' || s[i] == '{' || s[i] == '}' || s[i] == ',';

        bool IsQuote(string s, int i)
        {
            if (s[i] != '`')
                return false;

            WriteLine($"IsQuote @{i} '{s[i]}'");

            int j = i - 1, backslashCount = 0;
            while (j > 0 && s[j] == '\\')
            {
                j--;
                backslashCount++;
            }

            var result = backslashCount % 2 == 0;
            WriteLine($"IsQuote backslashCount {backslashCount} result {result}");

            return result;
        }

        bool IsSeparator(string s, int i) => char.IsWhiteSpace(s[i]) || IsMeta(s, i) || IsQuote(s, i);

        public List<DslToken> Tokenize(string s)
        {
            WriteLine($"Tokenizing: '{s}'");

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

                int start = pos;

                if (IsMeta(s, pos))
                {
                    Add(new DslToken(TokenKind.Meta, s[pos].ToString(), start, pos, configuration));
                    pos++;
                    continue;
                }

                if (IsQuote(s, pos))
                {
                    do
                    {
                        pos++;
                        if (pos >= s.Length)
                        {
                            var preview = StringUtl.PreviewString(s, start);
                            throw new InvalidOperationException($"Parse error. Unmatched quote starting at pos: {start}\n{preview}\n");
                        }
                    } while (!IsQuote(s, pos));

                    var substring = s.Substring(start + 1, pos - start - 1);
                    Add(new DslToken(TokenKind.String, substring, start, pos, configuration));
                    pos++;
                    continue;
                }

                do
                {
                    pos++;
                } while (pos < s.Length && !IsSeparator(s, pos));
                Add(new DslToken(TokenKind.Value, s.Substring(start, pos - start), start, pos, configuration));
                continue;
            }

            return tokens;

            void Add(DslToken t)
            {
                WriteLine($"New token {t}");
                tokens.Add(t);
            }
        }
    }
}