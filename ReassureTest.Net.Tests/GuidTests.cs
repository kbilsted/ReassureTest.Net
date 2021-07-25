using System;
using NUnit.Framework;

namespace ReassureTest.Tests
{
    public class GuidTests
    {
        [Test]
        public void Guid_with_rollingvalues_for_fuzzy_matching()
        {
            Guid g = Guid.NewGuid();
            g.Is("guid-0");

            // new run refreshes rolling value
            g = Guid.NewGuid();
            g.Is("guid-0");
        }

        [Test]
        public void Guids_with_rollingvalues_for_fuzzy_matching()
        {
            Tuple.Create(Guid.NewGuid(), Guid.NewGuid()).Is(@"
            {
                Item1 = guid-0
                Item2 = guid-1
            }");
        }

        [Test]
        public void Guid_with_rollingvalues_doesnt_match_wrong_rolling_value()
        {

            void act()
            {
                var g1 = Guid.NewGuid();
                var g2 = Guid.NewGuid();
                Tuple.Create(g1, g2)
                    .Is(@"
                    {
                        Item1 = guid-1
                        Item2 = guid-1
                    }");
            }

            var ex = Assert.Throws<AssertException>(act);
            StringAssert.StartsWith("Path: 'Item1'. Expected: <guid-1>\r\n  But was:  <guid-0>", ex.Message);
        }


        [Test]
        public void Guid_with_exactvalues_doesnt_match_rolling_guid()
        {
            void Act() => Guid.NewGuid().Is("guid-0", TestsSetup.ExactGuidValuesCfg);
            var ex = Assert.Throws<AssertException>(Act);

            StringAssert.StartsWith("Path: ''. Expected: <guid-0>\r\n  But was:  ", ex.Message);
        }

        [Test]
        public void Guid_with_exact_value_matches_that_value()
        {
            var g = Guid.Parse("99998888-e89b-12d3-a456-426614174000");
            g.Is("99998888-e89b-12d3-a456-426614174000", TestsSetup.ExactGuidValuesCfg);
        }

    }
}