using System;

namespace ReassureTest
{
    public class ReassureRunContext
    {
        private readonly object actual;
        private Action alternativeActual;

        private readonly Configuration configuration;

        public ReassureRunContext(Configuration configuration)
        {
            this.configuration = configuration.DeepClone();
        }

        public ReassureRunContext(object actual, Configuration configuration) : this(configuration)
        {
            this.actual = actual;
        }

        public string Is(string expected)
        {
            var target = actual ?? Reassure.Catch(alternativeActual);

            return target.Is(expected, configuration);
        }

        public ReassureRunContext Catch(Action actual)
        {
            alternativeActual = actual;
            return this;
        }

        public ReassureRunContext Catch<T>(Func<T> actual)
        {
            alternativeActual = () => actual();
            return this;
        }
    }
}