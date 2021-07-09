using System;
using System.Collections.Generic;
using System.Linq;
using ReassureTest.Net.AST;

namespace ReassureTest.Net
{
    public static class ReassureSetup
    {
        public static void Is_old(this object o, object actual)
        {
            var sb = new IndentingStringBuilder();
            new Visitor().Visit(sb, o);
            Console.WriteLine(sb);
        }

        public static void Is(this object o, object actual)
        {
            Is_old(o, actual);

            Console.WriteLine("!!!!!!!!!!!!!");
            Console.WriteLine("!!!!!!!!!!!!!");
            Console.WriteLine("!!!!!!!!!!!!!");

            var ast = new ObjectVisitor().Visit(o);
            Console.WriteLine(new AstPrinter().PrintRoot(ast));
        }

        /// <summary>
        /// excpected, actual
        /// </summary>
        public static Action<object, object> Assert { get; set; }
    }

    public class DSLParser
    {
        public enum TokenKind
        {
            String, Value, Meta
        }

        public class Token
        {
            public Token(TokenKind kind, string value)
            {
                Kind = kind;
                this.value = value;
            }

            public TokenKind Kind;
            public string value;

            public override string ToString() => "{" + Kind + ":" + value + "}";
        }

        bool IsMeta(string s, int i) => s[i] == '=' || s[i] == '[' || s[i] == ']' || s[i] == '{' || s[i] == '}';

        bool IsQuote(string s, int i) => i == 0 ? s[i] == '"' : s[i] == '"' && s[i - 1] != '\\';

        bool IsSeparator(string s, int i) => char.IsWhiteSpace(s[i]) || IsMeta(s, i) || IsQuote(s, i);

        public List<Token> Tokenize(string s)
        {
            var tokens = new List<Token>();

            int i = 0;
            while (i < s.Length)
            {
                if (char.IsWhiteSpace(s[i]))
                {
                    i++;
                    continue;
                }

                if (IsMeta(s, i))
                {
                    tokens.Add(new Token(TokenKind.Meta, s[i].ToString()));
                    i++;
                    continue;
                }

                int start = i;

                if (IsQuote(s, i))
                {
                    i++;
                    while (!IsQuote(s, i))
                        i++;
                    i++;
                    tokens.Add(new Token(TokenKind.String, s.Substring(start, i - start)));
                    continue;
                }

                while (!IsSeparator(s, i))
                    i++;
                tokens.Add(new Token(TokenKind.Value, s.Substring(start, i - start)));
                continue;
            }

            return tokens;
        }
    }
}
