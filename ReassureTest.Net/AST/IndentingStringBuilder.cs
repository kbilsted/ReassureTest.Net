using System;
using System.Text;

namespace ReassureTest.Net
{
    public class IndentingStringBuilder
    {
        private readonly StringBuilder sb = new StringBuilder();
        private const string indent = "    ";
        private int indentCount = 0;

        public IndentingStringBuilder Indent()
        {
            indentCount++;
            return this;
        }

        public IndentingStringBuilder Dedent()
        {
            if (indentCount == 0)
                throw new Exception("Indention level is 0, cannot decrease");
            indentCount--;
            return this;
        }

        public IndentingStringBuilder Add(object s)
        {
            sb.Append(s);
            return this;
        }

        public IndentingStringBuilder AddLine() => AddLine("");

        public IndentingStringBuilder AddLine(string s)
        {
            sb.AppendLine(s);
            return this;
        }

        public IndentingStringBuilder AddIndented(string s)
        {
            DoIndent();
            sb.Append(s);
            return this;
        }

        public IndentingStringBuilder AddLineIndented(string s)
        {
            DoIndent();
            sb.AppendLine(s);
            return this;
        }

        private void DoIndent()
        {
            for (int i = 0; i < indentCount; i++)
                sb.Append(indent);
        }

        public override string ToString()
        {
            return sb.ToString();
        }
    }
}