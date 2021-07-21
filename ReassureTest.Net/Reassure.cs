﻿using System;
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
        public static void Is(this object actual, string expected) => Is(actual, expected, CreateConfiguration());

        public static void Is(this object actual, string expected, Configuration cfg)
        {
            IValue astActual = new ObjectVisitor(cfg).Visit(actual);
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
                    DefaultConfiguration.Outputting.Indention,
                    DefaultConfiguration.Outputting.EnableDebugPrint,
                    DefaultConfiguration.Outputting.Print),
                new Configuration.AssertionCfg(
                    DefaultConfiguration.Assertion.Assert,
                    DefaultConfiguration.Assertion.DateTimeSlack,
                    DefaultConfiguration.Assertion.DateTimeFormat),
                new Configuration.HarvestingCfg(DefaultConfiguration.Harvesting.FieldValueTranslators)
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
                dateTimeFormat: "yyyy-MM-ddTHH:mm:ss"),
            new Configuration.HarvestingCfg(fieldValueTranslators: new List<Func<object, object>>()
            {
                FieldValueTranslatorImplementations.IsHarvestable,
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

        /// <summary>
        /// Tell if the type makes any sense to dump
        /// </summary>
        public static object IsHarvestable(object o)
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
        public SimplifiedException(Exception e)
        {
            Message = e.Message;
            Data = e.Data;
        }
    }
}