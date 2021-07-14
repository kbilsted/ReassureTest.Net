using System;

namespace ReassureTest.Net
{
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