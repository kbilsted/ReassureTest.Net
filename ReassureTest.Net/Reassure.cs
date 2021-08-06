using System;
using System.Collections.Generic;
using System.Reflection;
using ReassureTest.AST;
using ReassureTest.AST.Expected;
using ReassureTest.DSL;
using ReassureTest.Implementation;

namespace ReassureTest
{
    public static class Reassure
    {
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

        public static ReassureRunContext With(this object actual, Configuration configuration) => new ReassureRunContext(actual, configuration);

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
                projectors: new List<Configuration.HarvestingCfg.Projector>()
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