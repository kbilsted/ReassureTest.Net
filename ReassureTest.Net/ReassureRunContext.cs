namespace ReassureTest
{
    public class ReassureRunContext
    {
        private readonly object actual;
        private readonly Configuration configuration;

        public ReassureRunContext(object actual, Configuration configuration)
        {
            this.actual = actual;
            this.configuration = configuration.DeepClone();
        }

        public string Is(string expected) => actual.Is(expected, configuration);
    }
}