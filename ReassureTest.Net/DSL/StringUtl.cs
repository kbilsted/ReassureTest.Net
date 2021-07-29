using System;

namespace ReassureTest.DSL
{
    class StringUtl
    {
        private const int NumberOfCharacterContextOnDslParseError = 33;
        public static string PreviewString(string s, int start)
        {
            var prefix = Math.Max(0, start - NumberOfCharacterContextOnDslParseError);
            var postfix = Math.Min(start + NumberOfCharacterContextOnDslParseError, s.Length);

            return
                ((prefix > 0 ? "..." : "")
                + s.Substring(prefix, postfix - prefix)
               + (postfix < s.Length-1 ? "..." : ""))
                .Replace('\n', ' ')
                .Replace('\r', ' ')
                .Replace('\t', ' ');
        }
    }
}