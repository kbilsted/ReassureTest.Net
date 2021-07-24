namespace ReassureTest.AST.Expected
{
    class AstGuidMatcher : IAssertEvaluator
    {
        public AstSimpleValue UnderlyingValue { get; set; }

        public AstGuidMatcher(AstSimpleValue value)
        {
            UnderlyingValue = value;
        }
    }
}