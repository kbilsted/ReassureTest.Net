using NUnit.Framework;

namespace ReassureTest.Tests
{
    [SetUpFixture]
    public class TestsSetup
    {
        [OneTimeSetUp]
        public void Setup()
        {
            ReassureTest.Defaults.Assert = Assert.AreEqual;
            ReassureTest.Defaults.EnableDebugPrint = true;
        }
    }
}