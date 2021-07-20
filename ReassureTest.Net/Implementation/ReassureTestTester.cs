using System;
using ReassureTest.AST;
using ReassureTest.AST.Expected;
using ReassureTest.DSL;

namespace ReassureTest.Implementation
{
    /// <summary>
    /// Use this when testing ReassureTest.Net
    /// </summary>
    public class ReassureTestTester
    {
        public void Is(object actual, string expected) => Is(actual, expected, Defaults.CreateConfiguration());

        public void Is(object actual, string expected, Configuration cfg)
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
    }
}
