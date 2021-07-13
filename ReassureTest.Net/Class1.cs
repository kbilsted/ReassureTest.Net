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
        private readonly Tokenizer tokenizer;
        private int i = 0;
        Tokenizer.Token[] tokens;

        public DSLParser(Tokenizer tokenizer)
        {
            this.tokenizer = tokenizer;
        }

        public IValue Parse(string s)
        {
            tokens = tokenizer.Tokenize(s).ToArray();
            return ParseIValue();
        }

        bool PeekValueOrString()
        {
            Tokenizer.Token t = tokens[i];
            return t.Kind == Tokenizer.TokenKind.String || t.Kind == Tokenizer.TokenKind.Value;
        }

        void PeekEatMeta(string s)
        {
            if(PeekMeta(s))
                EatMeta(s);
        }

        bool PeekMeta(string s)
        {
            Tokenizer.Token t = tokens[i];
            return t.Kind == Tokenizer.TokenKind.Meta && t.value == s;
        }

        void EatMeta(string s)
        {
            if (!PeekMeta(s))
            {
                Tokenizer.Token t = tokens[i];
                throw new Exception($"Expected '{s}' got '{t.value}' of kind '{t.Kind}' at token: {i}");
            }
            i++;
        }

        string EatValue()
        {
            Tokenizer.Token t = tokens[i];
            if (t.Kind != Tokenizer.TokenKind.Value)
                throw new Exception($"Expected a word got '{t.value}' of kind '{t.Kind}' at token: {i}");
            i++;
            return t.value;
        }

        string EatValueOrString()
        {
            Tokenizer.Token t = tokens[i];
            if (t.Kind != Tokenizer.TokenKind.Value && t.Kind != Tokenizer.TokenKind.String)
                throw new Exception($"Expected a word or string got '{t.value}' of kind '{t.Kind}' at token: {i}");
            i++;
            return t.value;
        }

        public IValue ParseIValue()
        {
            if (PeekMeta("{"))
                return ParseComplex();
            if (PeekMeta("["))
                return ParseArray();
            if (PeekValueOrString())
                return ParseSimple();

            Tokenizer.Token t = tokens[i];
            throw new Exception($"Expected '{t.value}' of kind '{t.Kind}' at token: {i}");
        }

        private AstSimpleValue ParseSimple()
        {
            return new AstSimpleValue(EatValueOrString());
        }

        private IValue ParseArray()
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
            return array;
        }

        private IValue ParseComplex()
        {
            EatMeta("{");
            var c = new AstComplexValue();
            while (!PeekMeta("}"))
            {
                var name = EatValue();
                EatMeta("=");
                var value = ParseIValue();
                c.Values.Add(name, value);
            }
            EatMeta("}");
            return c;
        }
    }

    public class Tokenizer
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

        bool IsMeta(string s, int i) => s[i] == '=' || s[i] == '[' || s[i] == ']' || s[i] == '{' || s[i] == '}' || s[i] == ',';

        bool IsQuote(string s, int i)
        {
            if (s[i] != '"')
                return false;

            int j = i - 1, backslashCount = 0;
            while (j > 0 && s[j] == '\\')
            {
                j--;
                backslashCount++;
            }

            return backslashCount % 2 == 0;
        }

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
