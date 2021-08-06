using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace ReassureTest
{
    public class Configuration
    {
        public OutputtingCfg Outputting {get; set; }
        public AssertionCfg Assertion {get; set; }
        public HarvestingCfg Harvesting {get; set; }
        public TestFrameworkIntegratonCfg TestFrameworkIntegration {get; set; }

        public Configuration(OutputtingCfg outputting, AssertionCfg assertion, HarvestingCfg harvesting, TestFrameworkIntegratonCfg testFrameworkIntegration)
        {
            Outputting = outputting;
            Assertion = assertion;
            Harvesting = harvesting;
            TestFrameworkIntegration = testFrameworkIntegration;
        }

        public Configuration DeepClone()
        {
            return new Configuration(
                new OutputtingCfg(
                    Outputting.Indention,
                    Outputting.EnableDebugPrint
                ),
                new AssertionCfg(
                    Assertion.DateTimeSlack,
                    Assertion.DateTimeFormat,
                    Assertion.GuidHandling
                ),
                new HarvestingCfg(
                    Harvesting.Projectors
                ),
                new TestFrameworkIntegratonCfg(
                    TestFrameworkIntegration.RemapException,
                    TestFrameworkIntegration.Print
                )
            );
        }

        public enum GuidHandling
        {
            Exact, Rolling
        }

        public class AssertionCfg
        {
            public TimeSpan DateTimeSlack {get; set; }
            public string DateTimeFormat {get; set; }
            public GuidHandling GuidHandling {get; set; }

            public AssertionCfg(TimeSpan dateTimeSlack, string dateTimeFormat, GuidHandling guidHandling)
            {
                DateTimeSlack = dateTimeSlack;
                DateTimeFormat = dateTimeFormat;
                GuidHandling = guidHandling;
            }
        }

        public class HarvestingCfg
        {
            public List<Projector> Projectors { get; set; }

            /// <summary>Filter away fields or project their data to different values</summary>
            /// <param name="parent">the object holding the field</param>
            /// <param name="field">the value of the field</param>
            /// <param name="pi">Meta data on the field</param>
            public delegate Flow Projector(object parent, object field, PropertyInfo pi);

            public HarvestingCfg(List<Projector> projectors)
            {
                Projectors = new List<Projector>(projectors);
            }

            public HarvestingCfg Add(Projector p)
            {
                Projectors.Insert(0, p);
                return this;
            }
        }

        public class OutputtingCfg
        {
            public string Indention {get; set; }
            public bool EnableDebugPrint {get; set; }

            public OutputtingCfg(string indention, bool enableDebugPrint)
            {
                Indention = indention;
                EnableDebugPrint = enableDebugPrint;
            }
        }

        public class TestFrameworkIntegratonCfg
        {
            public Func<AssertException, Exception> RemapException {get; set; }
            public Action<string> Print {get; set; }

            public TestFrameworkIntegratonCfg(Func<AssertException, Exception> remapException, Action<string> print)
            {
                RemapException = remapException;
                Print = print;
            }
        }
    }
}