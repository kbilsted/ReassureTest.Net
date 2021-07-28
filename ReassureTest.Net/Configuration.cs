using System;
using System.Collections.Generic;

namespace ReassureTest
{
    public class Configuration
    {
        public OutputtingCfg Outputting;
        public AssertionCfg Assertion;
        public HarvestingCfg Harvesting;
        public TestFrameworkIntegratonCfg TestFrameworkIntegration;

        public Configuration(OutputtingCfg outputting, AssertionCfg assertion, HarvestingCfg harvesting, TestFrameworkIntegratonCfg testFrameworkIntegration)
        {
            Outputting = outputting;
            Assertion = assertion;
            Harvesting = harvesting;
            TestFrameworkIntegration = testFrameworkIntegration;
        }

        public static Configuration New()
        {
            return new Configuration(
                new OutputtingCfg(
                    Reassure.DefaultConfiguration.Outputting.Indention,
                    Reassure.DefaultConfiguration.Outputting.EnableDebugPrint
                ),
                new AssertionCfg(
                    Reassure.DefaultConfiguration.Assertion.DateTimeSlack,
                    Reassure.DefaultConfiguration.Assertion.DateTimeFormat,
                    Reassure.DefaultConfiguration.Assertion.GuidHandling
                ),
                new HarvestingCfg(
                    Reassure.DefaultConfiguration.Harvesting.FieldValueTranslators),
                new TestFrameworkIntegratonCfg(
                    Reassure.DefaultConfiguration.TestFrameworkIntegration.RemapException,
                    Reassure.DefaultConfiguration.TestFrameworkIntegration.Print
                )
            );
        }

        public enum GuidHandling
        {
            Exact, Rolling
        }

        public class AssertionCfg
        {
            public TimeSpan DateTimeSlack;
            public string DateTimeFormat;
            public GuidHandling GuidHandling;

            public AssertionCfg(TimeSpan dateTimeSlack, string dateTimeFormat, GuidHandling guidHandling)
            {
                DateTimeSlack = dateTimeSlack;
                DateTimeFormat = dateTimeFormat;
                GuidHandling = guidHandling;
            }
        }

        public class HarvestingCfg
        {
            public List<Func<object, object>> FieldValueTranslators;
         
            public HarvestingCfg(List<Func<object, object>> fieldValueTranslators)
            {
                FieldValueTranslators = fieldValueTranslators;
            }
        }

        public class OutputtingCfg
        {
            public string Indention;
            public bool EnableDebugPrint;

            public OutputtingCfg(string indention, bool enableDebugPrint)
            {
                Indention = indention;
                EnableDebugPrint = enableDebugPrint;
            }
        }

        public class TestFrameworkIntegratonCfg
        {
            public Func<AssertException, Exception> RemapException;
            public Action<string> Print;

            public TestFrameworkIntegratonCfg(Func<AssertException, Exception> remapException, Action<string> print)
            {
                RemapException = remapException;
                Print = print;
            }
        }
    }
}