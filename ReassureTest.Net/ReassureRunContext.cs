using System;

namespace ReassureTest
{
    public class ReassureRunContext
    {
        private readonly object? actual;
        private Action? alternativeActual;

        private readonly Configuration configuration;

        public ReassureRunContext(Configuration configuration)
        {
            this.configuration = configuration.DeepClone();
        }

        public ReassureRunContext(object actual, Configuration configuration) : this(configuration)
        {
            this.actual = actual;
        }

        public ReassureRunContext With(Projector p)
        {
            configuration.Harvesting.Add(p);
            return this;
        }

        public ReassureRunContext Without(WithoutPredicate p) => With(Reassure.ToProjector(p));

        public string Is(string expected)
        {
            var target = actual 
                         ?? Reassure.Catch(alternativeActual 
                                           ?? throw new InvalidOperationException("alternativeActual unexpectedly is null"));

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