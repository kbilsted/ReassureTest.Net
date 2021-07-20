using NUnit.Framework;

namespace ReassureTest.Tests
{
    [SetUpFixture]
    public class TestsSetup
    {
        [OneTimeSetUp]
        public void Setup()
        {
            ReassureTest.Reassure.Assert = Assert.AreEqual;
            ReassureTest.Reassure.EnableDebugPrint = true;
        }
    }
}