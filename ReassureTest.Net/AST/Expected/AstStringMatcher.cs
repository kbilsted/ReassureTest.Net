namespace ReassureTest.AST.Expected
{
    class AstStringMatcher : IAssertEvaluator
    {
        public AstSimpleValue UnderlyingValue { get; set; }
        
        public AstStringMatcher(AstSimpleValue value)
        {
            UnderlyingValue = value;
        }
    }
}