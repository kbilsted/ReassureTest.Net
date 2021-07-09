using System;
using System.Text;

namespace ReassureTest.Net
{
    class IndentingStringBuilder
    {
        private StringBuilder sb = new StringBuilder();
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
            for (int i = 0; i < indentCount; i++)
                sb.Append(indent);

            sb.Append(s);
            return this;
        }

        public IndentingStringBuilder AddLineIndented(string s)
        {
            for (int i = 0; i < indentCount; i++)
                sb.Append(indent);

            sb.AppendLine(s);
            return this;
        }

        public override string ToString()
        {
            return sb.ToString();
        }
    }
}