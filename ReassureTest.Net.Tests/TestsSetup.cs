using NUnit.Framework;

namespace ReassureTest.Tests
{
    [SetUpFixture]
    public class TestsSetup
    {
        [OneTimeSetUp]
        public void Setup()
        {
            ReassureTest.Reassure.DefaultConfiguration.Assertion.Assert = Assert.AreEqual;
            ReassureTest.Reassure.DefaultConfiguration.Outputting.EnableDebugPrint = true;
        }
    }
}