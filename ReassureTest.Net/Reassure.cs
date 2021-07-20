using System;
using ReassureTest.AST;
using ReassureTest.AST.Expected;
using ReassureTest.DSL;
using ReassureTest.Implementation;

namespace ReassureTest
{
    public static class Reassure
    {
        public static void Is(this object actual, string expected) => Is(actual, expected, CreateConfiguration());

        public static void Is(this object actual, string expected, Configuration cfg)
        {
            IValue astActual = new ObjectVisitor().Visit(actual);
            IValue expectedAst = new DslParser(new DslTokenizer(cfg), cfg).Parse(expected);

            if (expectedAst == null)
            {
                string graph = new AstPrinter(cfg).PrintRoot(astActual);
                cfg.Outputting.Print($@"Actual is:
{graph}");

                cfg.Assertion.Assert(graph, expected);
                return;
            }

            try
            {
                var executor = new MatchExecutor(cfg.Assertion.Assert);
                executor.MatchGraph(expectedAst as IAssertEvaluator, astActual);
            }
            catch (Exception)
            {
                string graph = new AstPrinter(cfg).PrintRoot(astActual);
                cfg.Outputting.Print($@"Actual is:
{graph}");
                throw;
            }
        }

        public static Configuration CreateConfiguration()
        {
            return new Configuration(
                new Configuration.OutputtingCfg(
                    Indention, 
                    EnableDebugPrint, 
                    Print),
                new Configuration.AssertionCfg(
                    Assert, 
                    DateTimeSlack, 
                    DateTimeFormat));
        }

        /// <summary>
        /// expected, actual
        /// </summary>
        public static Action</*expected*/object, /*actual*/object> Assert { get; set; }

        public static Action<string> Print { get; set; } = Console.WriteLine;
        
        public static string Indention = "    ";

        public static bool EnableDebugPrint = false;

        public static string DateTimeFormat = "yyyy-MM-ddTHH:mm:ss";

        public static TimeSpan DateTimeSlack = TimeSpan.FromSeconds(3);
    }
}