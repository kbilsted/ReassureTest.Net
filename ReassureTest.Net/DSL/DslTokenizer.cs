using System;
using System.Collections.Generic;

namespace ReassureTest.Net
{
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
}