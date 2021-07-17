using NUnit.Framework;

namespace ReassureTest.Tests
{
    [SetUpFixture]
    public class TestsSetup
    {
        [OneTimeSetUp]
        public void Setup()
        {
            ReassureTest.Setup.Assert = Assert.AreEqual;
            ReassureTest.Setup.EnableDebugPrint = true;
        }
    }
}