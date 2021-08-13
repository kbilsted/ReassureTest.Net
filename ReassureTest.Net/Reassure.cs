using System;
using System.Collections.Generic;
using System.Reflection;
using ReassureTest.AST;
using ReassureTest.AST.Expected;
using ReassureTest.DSL;
using ReassureTest.Implementation;

namespace ReassureTest
{
    /// <summary>
    /// return true to filter away field, otherwise use it unmodified
    /// </summary>
    public delegate bool WithoutPredicate(PropertyInfo pi);

    /// <summary>Filter away fields or project their data to different values</summary>
    /// <param name="parent">the object holding the field</param>
    /// <param name="field">the value of the field</param>
    /// <param name="pi">Meta data on the field</param>
    public delegate Flow Projector(object parent, object field, PropertyInfo pi);

    public static class Reassure
    {
        /// <summary>
        /// Run code and catch the exception
        /// </summary>
        public static Exception Catch(Action actual)
        {
            try
            {
                actual();
            }
            catch (Exception e)
            {
                return e;
            }
            
            throw new AssertException($"Expected: an exception to be thrown\r\nBut no exception was thrown");
        }

        /// <summary>
        /// Run code and catch the exception
        /// </summary>
        public static Exception Catch<T>(Func<T> actual)
        {
            try
            {
                actual();
            }
            catch (Exception e)
            {
                return e;
            }

            throw new AssertException($"Expected: an exception to be thrown\r\nBut no exception was thrown");
        }

        /// <summary>
        /// Assert 'actual' obeys the specification using the default configuration
        /// </summary>
        public static string Is(this object actual, string expected) => Is(actual, expected, DefaultConfiguration.DeepClone());

        internal static string Is(this object actual, string expected, Configuration cfg)
        {
            IAstNode astActual = new ObjectVisitor(cfg).VisitRoot(actual);
            IAstNode expectedAst = new DslParser(new DslTokenizer(cfg), cfg).Parse(expected);

            string graph = new AstPrinter(cfg).PrintRoot(astActual);

            if (astActual == null && expectedAst == null)
                return graph;
            
            try
            {
                if (astActual == null && expectedAst != null)
                    throw new AssertException($"Expected: {expected}\r\nBut was:  <empty>    (all fields have been filtered away)");
                
                if (expectedAst == null)
                {
                    MatchExecutor.Compare(expected, actual, "", cfg);
                }
                else
                {
                    var executor = new MatchExecutor(cfg);
                    executor.MatchGraph(expectedAst as IAssertEvaluator, astActual);
                }
            }
            catch (Exception e)
            {
                e.Data.Add("Actual", graph);
                cfg.TestFrameworkIntegration.Print($"Actual is:\r\n{graph}");
                if (e is AssertException ae)
                    throw cfg.TestFrameworkIntegration.RemapException(ae);
                throw;
            }

            return graph;
        }

        /// <summary>
        /// inject a configuration into a throws
        /// </summary>
        public static ReassureRunContext With(Configuration configuration) => new ReassureRunContext(configuration);

        /// <summary>
        /// inject a configuration 
        /// </summary>
        public static ReassureRunContext With(this object actual, Configuration configuration) => new ReassureRunContext(actual, configuration);

        /// <summary>
        /// inject a clone of the default configuration + projector
        /// </summary>
        public static ReassureRunContext With(this object actual, Projector projector)
        {
            var cfg = Reassure.DefaultConfiguration.DeepClone();
            cfg.Harvesting.Add(projector);
            return new ReassureRunContext(actual, cfg);
        }

        /// <summary>
        /// inject a clone of the default configuration + filter
        /// </summary>
        public static ReassureRunContext Without(this object actual, WithoutPredicate predicate)
            => With(actual, Configuration.HarvestingCfg.ToProjector(predicate));

        /// <summary>
        /// Alter this to change the general behaviour of ReassureTest
        /// </summary>
        public static Configuration DefaultConfiguration = new Configuration(
            new Configuration.OutputtingCfg(
                indention: "    ",
                enableDebugPrint: false
            ),
            new Configuration.AssertionCfg(
                dateTimeSlack: TimeSpan.FromSeconds(3),
                dateTimeFormat: "yyyy-MM-ddTHH:mm:ss",
                guidHandling: Configuration.GuidHandling.Rolling
            ),
            new Configuration.HarvestingCfg(
                projectors: new List<Projector>()
                {
                    ReusableProjections.FixDefaultImmutableArrayCanNotBeTraversed,
                    ReusableProjections.SimplifyExceptions,
                    ReusableProjections.SkipUnharvestableTypes,
                }
            ),
            new Configuration.TestFrameworkIntegratonCfg(
                remapException: ex => ex,
                print: Console.WriteLine
            )
        );
    }
}