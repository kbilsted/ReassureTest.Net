using System;
using System.Collections.Generic;

namespace ReassureTest
{
    public class Configuration
    {
        public readonly OutputtingCfg Outputting;
        public readonly AssertionCfg Assertion;
        public readonly HarvestingCfg Harvesting;
        public readonly TestFrameworkIntegratonCfg TestFrameworkIntegration;

        public Configuration(OutputtingCfg outputting, AssertionCfg assertion, HarvestingCfg harvesting, TestFrameworkIntegratonCfg testFrameworkIntegration)
        {
            Outputting = outputting;
            Assertion = assertion;
            Harvesting = harvesting;
            TestFrameworkIntegration = testFrameworkIntegration;
        }

        public enum GuidHandling
        {
            Exact, Rolling
        }

        public class AssertionCfg
        {
            public TimeSpan DateTimeSlack { get; set; }
            public string DateTimeFormat { get; set; }
            public GuidHandling GuidHandling { get; set; }

            public AssertionCfg(TimeSpan dateTimeSlack, string dateTimeFormat, GuidHandling guidHandling)
            {
                DateTimeSlack = dateTimeSlack;
                DateTimeFormat = dateTimeFormat;
                GuidHandling = guidHandling;
            }
        }

        public class HarvestingCfg
        {
            public List<Func<object, object>> FieldValueTranslators { get; set; }
         
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
    }

    public class TestFrameworkIntegratonCfg  
    {
        public Func<AssertException, Exception> RemapException { get; set; }
        public Action<string> Print;

        public TestFrameworkIntegratonCfg(Func<AssertException, Exception> remapException, Action<string> print)
        {
            RemapException = remapException;
            Print = print;
        }
    }
}