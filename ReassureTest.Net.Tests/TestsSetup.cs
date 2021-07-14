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
            ReassureSetup.Assert = Assert.AreEqual;
        }
    }
}