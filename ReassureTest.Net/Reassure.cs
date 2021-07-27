using System;
using System.Collections;
using System.Collections.Generic;
using ReassureTest.AST;
using ReassureTest.AST.Expected;
using ReassureTest.DSL;
using ReassureTest.Implementation;

namespace ReassureTest
{
    public static class Reassure
    {
        public static string Is(this object actual, string expected) => Is(actual, expected, CreateConfiguration());

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

        public static Configuration CreateConfiguration()
        {
            return new Configuration(
                new Configuration.OutputtingCfg(
                    DefaultConfiguration.Outputting.Indention,
                    DefaultConfiguration.Outputting.EnableDebugPrint
                    ),
                new Configuration.AssertionCfg(
                    DefaultConfiguration.Assertion.DateTimeSlack,
                    DefaultConfiguration.Assertion.DateTimeFormat,
                    DefaultConfiguration.Assertion.GuidHandling
                    ),
                new Configuration.HarvestingCfg(
                    DefaultConfiguration.Harvesting.FieldValueTranslators),
                new TestFrameworkIntegratonCfg(
                    DefaultConfiguration.TestFrameworkIntegration.RemapException,
                    DefaultConfiguration.TestFrameworkIntegration.Print
                    )
                );
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
            new TestFrameworkIntegratonCfg(
                remapException: ex => ex,
                print: Console.WriteLine
            )
        );
    }

    public static class FieldValueTranslatorImplementations
    {
        public static object SimplifyExceptions(object o)
        {
            if (o is Exception ex)
                return new SimplifiedException(ex);
            return o;
        }

        public static object IgnoreUnharvestableTypes(object o)
        {
            var typename = o.GetType().ToString();
            if (typename.StartsWith("System.Reflection", StringComparison.Ordinal)
                || typename.StartsWith("System.Runtime", StringComparison.Ordinal)
                || typename.StartsWith("System.SignatureStruct", StringComparison.Ordinal)
                || typename.StartsWith("System.Func", StringComparison.Ordinal))
                return null;
            return o;
        }
    }

    public class SimplifiedException
    {
        public string Message { get; set; }
        public IDictionary Data { get; set; }
        public string Type { get; set; }

        public SimplifiedException(Exception e)
        {
            Message = e.Message;
            Data = e.Data;
            Type = e.GetType().ToString();
        }
    }
}