using NUnit.Framework;

namespace ReassureTest.Tests
{
    public class NestedTests
    {
        [Test]
        public void Nested()
        {
            var o = new NestedTop()
            {
                A = new NestedChildA()
                {
                    I = 33
                },
                B = new NestedChildB()
                {
                    B = true
                },
                C = new NestedChildC()
                {
                    D = new NestedChildChildD() { G = 5 },
                    S = "some string"
                },
                S2 = "s2s2s2"
            };
            o.Is(@"{
    A = {
        I = 33
    }
    B = {
        B = true
    }
    C = {
        S = `some string`
        D = {
            G = *
        }
    }
    S2 = `s2s2s2`
}");
        }

        [Test]
        public void Nested_null()
        {
            var o = new NestedTop();
            var ex = Assert.Throws<AssertException>(()=>o.Is(@"{ A = { I = 33 }}"));
            Assert.AreEqual("Path: 'A'. Expected: not null\r\nBut was: null", ex.Message);
        }

        [Test]
        public void Nestedarray()
        {
            var o = new NestedTopArray()
            {
                C = new[]
                {
                    new NestedChildC()
                    {
                        D = new NestedChildChildD() { G = 3 },
                        S = "some string"
                    },new NestedChildC()
                    {
                        D = new NestedChildChildD() { G = 4 },
                        S = "hello world"
                    }
                }
            };
            o.Is(@"{
    C = [
        {
            S = `some string`
            D = {
                G = 3
            }
        },
        {
            S = `hello world`
            D = {
                G = 4
            }
        }
    ]
}");
        }


        class NestedTop
        {
            public NestedChildA A { get; set; }
            public NestedChildB B { get; set; }
            public NestedChildC C { get; set; }
            public string S2 { get; set; }
        }

        class NestedTopArray
        {
            public NestedChildC[] C { get; set; }
        }

        class NestedChildA
        {
            public int I { get; set; }
        }

        class NestedChildB
        {
            public bool B { get; set; }
        }

        class NestedChildC
        {
            public string S { get; set; }
            public NestedChildChildD D { get; set; }
        }

        class NestedChildChildD
        {
            public int G { get; set; }
        }
    }
}