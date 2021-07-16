using System;
using ReassureTest.Net.AST;

namespace ReassureTest.Net.DSL
{
    public class DslParser
    {
        private readonly DslTokenizer tokenizer;
        private int i = 0;
        DslToken[] tokens;
        private string input;

        public DslParser(DslTokenizer tokenizer)
        {
            this.tokenizer = tokenizer;
        }

        public IValue Parse(string s)
        {
            input = s;
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
                throw new Exception($"Expected '{s}' got '{t.Value}' position: {t.PosStart} (of kind '{t.Kind}' at token: {i})\n{StringUtl.PreviewString(input, t.PosStart)}");
            }
            i++;
        }

        object EatValue()
        {
            DslToken t = tokens[i];
            if (t.Kind != DslTokenizer.TokenKind.Value)
                throw new Exception($"Expected a word got '{t.Value}' position: {t.PosStart} (of kind '{t.Kind}' at token: {i})\n{StringUtl.PreviewString(input, t.PosStart)}");
            i++;
            return t.Value;
        }

        object EatValueOrString()
        {
            DslToken t = tokens[i];
            if (t.Kind != DslTokenizer.TokenKind.Value && t.Kind != DslTokenizer.TokenKind.String)
                throw new Exception($"Expected a word or string got '{t.Value}' position: {t.PosStart} (of kind '{t.Kind}' at token: {i})\n{StringUtl.PreviewString(input, t.PosStart)}");
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
            throw new Exception($"Unparseable '{t.Value}' position: {t.PosStart} (of kind '{t.Kind}' at token: '{i}')\n{StringUtl.PreviewString(input, t.PosStart)}");
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
                    case "now":
                        return new AstDateTimeMatcher(new AstSimpleValue(DateTime.Now), Setup.DateTimeSlack);
                }
            }
            else if (token is DateTime datetime)
                return new AstDateTimeMatcher(new AstSimpleValue(token), Setup.DateTimeSlack);

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
}