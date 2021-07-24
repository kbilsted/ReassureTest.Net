using NUnit.Framework;

namespace ReassureTest.Tests
{
    [SetUpFixture]
    public class TestsSetup
    {
        public static Configuration ExactGuidValuesCfg
        {
            get
            {
                var c = Reassure.CreateConfiguration();
                c.Assertion.GuidHandling = Configuration.GuidHandling.Exact;
                return c;
            }
        }

        [OneTimeSetUp]
        public void Setup()
        {
            ReassureTest.Reassure.DefaultConfiguration.Assertion.Assert = Assert.AreEqual;
            ReassureTest.Reassure.DefaultConfiguration.Outputting.EnableDebugPrint = true;
        }
    }
}