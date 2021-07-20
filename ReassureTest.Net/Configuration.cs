using System;

namespace ReassureTest
{
    public class Configuration
    {
        public readonly OutputtingCfg Outputting;
        public readonly AssertionCfg Assertion;

        public Configuration(OutputtingCfg outputting, AssertionCfg assertion)
        {
            this.Outputting = outputting;
            this.Assertion = assertion;
        }

        public class AssertionCfg
        {
            public Action</*expected*/object, /*actual*/object> Assert { get; set; }
            public TimeSpan DateTimeSlack { get; set; }
            public string DateTimeFormat = "yyyy-MM-ddTHH:mm:ss";

            public AssertionCfg(Action<object, object> assert, TimeSpan dateTimeSlack, string dateTimeFormat)
            {
                Assert = assert;
                this.DateTimeSlack = dateTimeSlack;
                this.DateTimeFormat = dateTimeFormat;
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