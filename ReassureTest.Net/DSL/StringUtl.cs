using System;

namespace ReassureTest.DSL
{
    class StringUtl
    {
        public static string PreviewString(string s, int start)
        {
            var prefix = Math.Max(0, start - 33);
            var postfix = Math.Min(start + 33, s.Length);

            return
                (prefix > 0 ? "..." : "")
                + s.Substring(prefix, postfix - prefix)
                    .Replace('\n', ' ')
                    .Replace('\r', ' ')
                    .Replace('\t', ' ')
                + "...";
        }
    }
}