namespace ReassureTest.AST.Expected
{
    class AstArrayMatcher : IAssertEvaluator
    {
        public AstArrayMatcher(AstArray value)
        {
            Value = value;
        }

        public AstArray Value { get; set; }
    }
}