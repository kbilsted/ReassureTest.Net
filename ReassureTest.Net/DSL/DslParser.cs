using System;
using System.Text.RegularExpressions;
using ReassureTest.AST;
using ReassureTest.AST.Expected;

namespace ReassureTest.DSL
{
    public class DslParser
    {
        private readonly DslTokenizer tokenizer;
        private readonly Configuration configuration;
        private int i = 0;
        DslToken[] tokens;
        private string input;

        public DslParser(DslTokenizer tokenizer, Configuration configuration)
        {
            this.tokenizer = tokenizer;
            this.configuration = configuration;
        }

        public IAstNode Parse(string s)
        {
            if (s == null)
                return null;
            input = s;
            tokens = tokenizer.Tokenize(s).ToArray();
            if (tokens.Length == 0)
                return null;
            return ParseAstNode();
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
                throw new InvalidOperationException($"Parse error. Expected '{s}', but got '{t.Value}' position: {t.PosStart} (of kind '{t.Kind}' at token: {i})\r\n{StringUtl.PreviewString(input, t.PosStart)}");
            }
            i++;
        }

        object EatValue()
        {
            DslToken t = tokens[i];
            if (t.Kind != DslTokenizer.TokenKind.Value)
                throw new InvalidOperationException($"Parse error. Expected a fieldname, but got '{t.Value}' position: {t.PosStart} (of kind '{t.Kind}' at token: {i})\r\n{StringUtl.PreviewString(input, t.PosStart)}");
            i++;
            return t.Value;
        }

        object EatValueOrString()
        {
            DslToken t = tokens[i];
            if (t.Kind != DslTokenizer.TokenKind.Value && t.Kind != DslTokenizer.TokenKind.String)
                throw new InvalidOperationException($"Parse error. Expected a fieldname or a string, but got '{t.Value}' position: {t.PosStart} (of kind '{t.Kind}' at token: {i})\r\n{StringUtl.PreviewString(input, t.PosStart)}");
            i++;
            return t.Value;
        }

        public IAssertEvaluator ParseAstNode()
        {
            if (PeekMeta("{"))
                return ParseComplex();
            if (PeekMeta("["))
                return ParseArray();
            if (PeekValueOrString())
                return ParseSimple();

            DslToken t = tokens[i];
            throw new InvalidOperationException($"Parse error. Expecting either '{{', '[', or a value. Can not accept '{t.Value}' at position: {t.PosStart} (of token kind '{t.Kind}' at token number: '{i}')\r\n{StringUtl.PreviewString(input, t.PosStart)}");
        }

        private static readonly Regex RollingGuidEx = new Regex("guid-(?<id>\\d+)", RegexOptions.Compiled);

        private IAssertEvaluator ParseSimple()
        {
            var token = EatValueOrString();

            if (token is string str)
            {
                var match = RollingGuidEx.Match(str);
                if (match.Success)
                {
                    var id = int.Parse(match.Groups["id"].Value);
                    return new AstGuidMatcher(new AstRollingGuid(id));
                }

                switch (str)
                {
                    case "?":
                        return new AstAnyMatcher();
                    case "*":
                        return new AstSomeMatcher();
                    case "now":
                        return new AstDateTimeMatcher(new AstSimpleValue(DateTime.Now), configuration.Assertion.DateTimeSlack);
                }
            }
            else if (token is DateTime)
                return new AstDateTimeMatcher(new AstSimpleValue(token), configuration.Assertion.DateTimeSlack);

            return new AstSimpleMatcher(new AstSimpleValue(token));
        }

        private IAssertEvaluator ParseArray()
        {
            EatMeta("[");
            var array = new AstArray();
            while (!PeekMeta("]"))
            {
                var value = ParseAstNode();
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
                var value = ParseAstNode();
                c.Values.Add(name, value);
            }
            EatMeta("}");
            return new AstComplexMatcher(c);
        }
    }
}