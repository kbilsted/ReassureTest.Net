namespace ReassureTest.AST.Expected
{
    /// <summary>
    /// An exact matcher, eg. i=3
    /// </summary>
    class AstSimpleMatcher : IAssertEvaluator
    {
        public AstSimpleValue UnderlyingValue { get; set; }

        public AstSimpleMatcher(AstSimpleValue value)
        {
            UnderlyingValue = value;
        }
    }
}