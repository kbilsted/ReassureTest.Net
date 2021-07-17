namespace ReassureTest.AST.Expected
{
    /// <summary>
    /// An eg. {a=2,b=3} = {a=2,b=3}
    /// </summary>
    class AstComplexMatcher : IAssertEvaluator
    {
        public AstComplexMatcher(AstComplexValue value)
        {
            Value = value;
        }

        public AstComplexValue Value { get; set; }
    }
}