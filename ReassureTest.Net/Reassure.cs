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

            if (expectedAst == null)
            {
                cfg.Outputting.Print($@"Actual is:
{graph}");

                cfg.Assertion.Assert(graph, expected);
                return graph;
            }

            try
            {
                var executor = new MatchExecutor(cfg.Assertion.Assert);
                executor.MatchGraph(expectedAst as IAssertEvaluator, astActual);
            }
            catch (Exception)
            {
                cfg.Outputting.Print($@"Actual is:
{graph}");
                throw;
            }

            return graph;
        }

        public static Configuration CreateConfiguration()
        {
            return new Configuration(
                new Configuration.OutputtingCfg(
                    DefaultConfiguration.Outputting.Indention,
                    DefaultConfiguration.Outputting.EnableDebugPrint,
                    DefaultConfiguration.Outputting.Print),
                new Configuration.AssertionCfg(
                    DefaultConfiguration.Assertion.Assert,
                    DefaultConfiguration.Assertion.DateTimeSlack,
                    DefaultConfiguration.Assertion.DateTimeFormat,
                    DefaultConfiguration.Assertion.GuidHandling),
                new Configuration.HarvestingCfg(
                    DefaultConfiguration.Harvesting.FieldValueTranslators)
                );
        }

        public static Configuration DefaultConfiguration = new Configuration(
            new Configuration.OutputtingCfg(
                indention: "    ",
                enableDebugPrint: false,
                print: Console.WriteLine),
            new Configuration.AssertionCfg(
                assert: null,
                dateTimeSlack: TimeSpan.FromSeconds(3),
                dateTimeFormat: "yyyy-MM-ddTHH:mm:ss",
                guidHandling: Configuration.GuidHandling.Rolling),
            new Configuration.HarvestingCfg(
                fieldValueTranslators: new List<Func<object, object>>()
            {
                FieldValueTranslatorImplementations.IgnoreUnharvestableTypes,
                FieldValueTranslatorImplementations.SimplifyExceptions
            }));
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