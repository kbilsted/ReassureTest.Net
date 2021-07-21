using System;
using System.Collections.Generic;

namespace ReassureTest
{
    public class Configuration
    {
        public readonly OutputtingCfg Outputting;
        public readonly AssertionCfg Assertion;
        public readonly HarvestingCfg Harvesting;

        public Configuration(OutputtingCfg outputting, AssertionCfg assertion, HarvestingCfg harvesting)
        {
            Outputting = outputting;
            Assertion = assertion;
            Harvesting = harvesting;
        }

        public class AssertionCfg
        {
            public Action</*expected*/object, /*actual*/object> Assert { get; set; }
            public TimeSpan DateTimeSlack { get; set; }
            public string DateTimeFormat = "yyyy-MM-ddTHH:mm:ss";

            public AssertionCfg(Action<object, object> assert, TimeSpan dateTimeSlack, string dateTimeFormat)
            {
                Assert = assert;
                DateTimeSlack = dateTimeSlack;
                DateTimeFormat = dateTimeFormat;
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
            public Action<string> Print;
            public string Indention;
            public bool EnableDebugPrint;

            public OutputtingCfg(string indention, bool enableDebugPrint, Action<string> print)
            {
                Indention = indention;
                EnableDebugPrint = enableDebugPrint;
                Print = print;
            }
        }
    }
}