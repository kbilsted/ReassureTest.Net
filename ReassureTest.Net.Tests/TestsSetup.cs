using System;
using NUnit.Framework;

namespace ReassureTest.Net.Tests
{
    [SetUpFixture]
    public class TestsSetup
    {
        [OneTimeSetUp]
        public void Setup()
        {
            ReassureTest.Net.Setup.Assert = Assert.AreEqual;
            ReassureTest.Net.Setup.EnableDebugPrint = true;
        }
    }
}