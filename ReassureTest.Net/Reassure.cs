using System;
using System.Collections.Generic;
using ReassureTest.AST;
using ReassureTest.AST.Expected;
using ReassureTest.DSL;
using ReassureTest.Implementation;

namespace ReassureTest
{
    public static class Reassure
    {
        public static string Is(this object actual, string expected) => Is(actual, expected, Configuration.New());

        public static string Is(this object actual, string expected, Configuration cfg)
        {
            IValue astActual = new ObjectVisitor(cfg).Visit(actual);
            IValue expectedAst = new DslParser(new DslTokenizer(cfg), cfg).Parse(expected);

            string graph = new AstPrinter(cfg).PrintRoot(astActual);

            try
            {
                if (expectedAst == null)
                {
                    MatchExecutor.Compare(graph, expected, "", cfg);
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
                cfg.TestFrameworkIntegration.Print($@"Actual is:
{graph}");
                if (e is AssertException ae)
                    throw cfg.TestFrameworkIntegration.RemapException(ae);
                throw;
            }

            return graph;
        }

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
                fieldValueTranslators: new List<Func<object, object>>()
                {
                    FieldValueTranslatorImplementations.IgnoreUnharvestableTypes,
                    FieldValueTranslatorImplementations.SimplifyExceptions
                }),
            new Configuration.TestFrameworkIntegratonCfg(
                remapException: ex => ex,
                print: Console.WriteLine
            )
        );
    }
}