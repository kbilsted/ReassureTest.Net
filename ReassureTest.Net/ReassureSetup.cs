using System;

namespace ReassureTest.Net
{
    public static class ReassureSetup
    {
        public static void Is(this object actual, string expected)
        {
            new ReassureTestTester().Is(actual, expected, Print, Assert);
        }

        /// <summary>
        /// excpected, actual
        /// </summary>
        public static Action</*expected*/object, /*actual*/object> Assert { get; set; }

        public static Action<string> Print { get; set; } = Console.WriteLine;

        public static string Indention = "    ";

        public static bool EnableDebugPrint = false;

    }
}