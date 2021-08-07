using System;

namespace ReassureTest.AST.Expected
{
    class AstDateTimeMatcher : IAssertEvaluator
    {
        public AstSimpleValue UnderlyingValue { get; set; }
        public readonly TimeSpan AcceptedSlack;

        public AstDateTimeMatcher(AstSimpleValue value, TimeSpan acceptedSlack)
        {
            UnderlyingValue = value;
            AcceptedSlack = acceptedSlack;
        }
    }
}