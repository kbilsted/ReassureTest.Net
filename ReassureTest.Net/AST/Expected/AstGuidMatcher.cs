namespace ReassureTest.AST.Expected
{
    class AstGuidMatcher : IAssertEvaluator
    {
        public AstRollingGuid UnderlyingValue { get; set; }

        public AstGuidMatcher(AstRollingGuid guid)
        {
            UnderlyingValue = guid;
        }
    }
}