using System;
using System.Text;

namespace ReassureTest.Net.AST
{
    public class IndentingStringBuilder
    {
        private readonly StringBuilder sb = new StringBuilder();
        private readonly string indent;
        private int indentCount = 0;

        public IndentingStringBuilder(string indent)
        {
            this.indent = indent;
        }

        public IndentingStringBuilder Indent()
        {
            indentCount++;
            return this;
        }

        public IndentingStringBuilder Dedent()
        {
            if (indentCount == 0)
                throw new Exception("Indention level can not go below 0");
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