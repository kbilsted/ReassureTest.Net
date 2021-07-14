using System;

namespace ReassureTest.Net.DSL
{
    class StringUtl
    {
        public static string PreviewString(string s, int start)
        {
            var prefix = Math.Max(0, start - 33);
            var postfix = Math.Min(start + 33, s.Length);

            return
                (prefix > 0 ? "..." : "")
                + s[new Range(prefix, postfix)]
                    .Replace('\n', ' ')
                    .Replace('\r', ' ')
                    .Replace('\t', ' ')
                + "...";
        }
    }
}