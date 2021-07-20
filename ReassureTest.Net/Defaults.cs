using System;
using ReassureTest.Implementation;

namespace ReassureTest
{
    public static class Defaults
    {
        public static void Is(this object actual, string expected)
        {
            var cfg = CreateConfiguration();
            new ReassureTestTester().Is(actual, expected, cfg);
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
        /// excpected, actual
        /// </summary>
        public static Action</*expected*/object, /*actual*/object> Assert { get; set; }

        public static Action<string> Print { get; set; } = Console.WriteLine;
        
        public static string Indention = "    ";

        public static bool EnableDebugPrint = false;

        public static string DateTimeFormat = "yyyy-MM-ddTHH:mm:ss";

        public static TimeSpan DateTimeSlack = TimeSpan.FromSeconds(3);
    }
}