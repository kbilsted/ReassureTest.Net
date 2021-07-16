using System;
using System.Globalization;
using ReassureTest.Net.DSL;

namespace ReassureTest.Net.AST
{
    public class AstPrinter
    {
        public string PrintRoot(IValue value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var sb = new IndentingStringBuilder(Setup.Indention);
            PrintIValue(value, sb);
            return sb.ToString();
        }

        void PrintIValue(IValue value, IndentingStringBuilder sb)
        {
            if (value is AstSimpleValue simpleValue)
                PrintSimple(simpleValue, sb);
            else if (value is AstArray astArray)
                PrintArray(astArray, sb);
            else if (value is AstComplexValue complex)
                PrintComplex(complex, sb);
            else
                throw new Exception($"this can never happen, please report as a bug\nv: {value} is of type: {value.GetType()}");
        }

        void PrintComplex(AstComplexValue value, IndentingStringBuilder sb)
        {
            sb.AddLine("{").Indent();
            foreach (var c in value.Values)
            {
                sb.AddIndented($"{c.Key} = ");
                PrintIValue(c.Value, sb);
                sb.AddLine();
            }
            sb.Dedent().AddIndented("}");
        }

        private void PrintArray(AstArray astArray, IndentingStringBuilder sb)
        {
            var count = astArray.Values.Count;
            
            bool printAsSingleline = astArray.ArrayKind == ValueKind.Simple;
            if (printAsSingleline)
            {
                sb.Add("[ ");
                for (int i = 0; i < count; i++)
                {
                    PrintIValue(astArray.Values[i], sb);
                    sb.Add(i < count - 1 ? ", " : "");
                }
                sb.Add(" ]");
            }
            else
            {
                sb.AddLine("[").Indent();

                for (int i = 0; i < count; i++)
                {
                    sb.AddIndented("");
                    PrintIValue(astArray.Values[i], sb);
                    sb.AddLine(i < count - 1 ? "," : "");
                }
                sb.Dedent().AddIndented("]");
            }
        }

        void PrintSimple(AstSimpleValue v, IndentingStringBuilder sb)
        {
            if (v.Value is string str)
            {
                sb.Add($"`{str}`");
            }
            else if (v.Value is bool b)
            {
                sb.Add(b.ToString().ToLower(CultureInfo.InvariantCulture));
            }
            else if (v.Value is DateTime dateTime)
            {
                sb.Add(dateTime.ToString(Setup.DateTimeFormat));
            }
            else
            {
                sb.Add(v.Value ?? "null");
            }
        }

    }
}