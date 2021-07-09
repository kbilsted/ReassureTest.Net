using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework;
using ReassureTest.Net.AST;
using System.Linq;

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

    public class Tests2
    {
        [Test]
        public void SimpleAsserts()
        {
            new DateTime(2022, 3, 4, 5, 6, 7).Is(new DateTime(2022, 3, 4, 5, 6, 7));
            (TimeSpan.FromMinutes(1) + TimeSpan.FromMinutes(2)).Is(TimeSpan.FromMinutes(3));
            Guid g = Guid.NewGuid();
            1.Is(1);
            2L.Is(2L);
            true.Is(true);
            "ss".Is("ss");
            g.Is(g);
        }

        [Test]
        public void NonNestedObject()
        {
            var simpleTypes = new SimpleTypes()
            {
                I = 42,
                L = 42978239382333L,
                B = true,
                S = "hello world",
                S2 = "hello \"Quotes\""
            };
            simpleTypes.Is("");

            // token

            var ast = new ObjectVisitor().Visit(simpleTypes);
            string result = new AstPrinter().PrintRoot(ast);
            var ts = new DSLParser().Parse(result);
            Console.WriteLine("------------------");
            Console.WriteLine("------------------");
            Console.WriteLine("------------------");
            Console.WriteLine(string.Join(", ", ts));
        }

        [Test]
        public void NonNestedObject_with_array()
        {
            new SimpleTypesArrays()
            {
                I = new[]{42,43},
                I2 = new []{new []{1,2,3}, new []{4,5}},
                L = new[]{42978239382333L},
                B = new[]{true, false},
                S = new[]{"hello world"},
            }.Is("");
        }


        [Test]
        public void NonNestedObject_with_dictionary()
        {
            new SimpleTypesDictionaries()
            {
                I = new Dictionary<int, int>() { {42, 43 }, { 111, 222 } },
                L = new Dictionary<long, int>() { {42978239382333L,1 }},
                B = new Dictionary<bool, int>(){ {true,2 }},
                S = new Dictionary<string, int>(){ {"hello world",3 }},
            }.Is("");
        }
        [Test]
        public void NonNestedObject_is_null()
        {
            SimpleTypes st = null;
            st.Is("");
        }


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
                    D = new NestedChildChildD() {G = Guid.NewGuid()},
                    S = "some string"
                },
                S2 = "s2s2s2"
            };
            o.Is("");
        }

        [Test]
        public void Nestedarray()
        {
            var o = new NestedTopArray()
            {
                C = new NestedChildC[]
                {
                    new NestedChildC()
                    {
                        D = new NestedChildChildD() { G = Guid.NewGuid() },
                        S = "some string"
                    },new NestedChildC()
                    {
                        D = new NestedChildChildD() { G = Guid.NewGuid() },
                        S = "hello world"
                    }
                }
            };
            o.Is("");
        }


        class SimpleTypes
        {
            public int I { get; set; }
            public long L { get; set; }
            public bool B { get; set; }
            public string S { get; set; }
            public string S2 { get; set; }
        }

        class SimpleTypesArrays
        {
            public int[] I { get; set; }
            public int[][] I2 { get; set; }
            public long[] L { get; set; }
            public bool[] B { get; set; }
            public string[] S { get; set; }
        }

        class SimpleTypesDictionaries
        {
            public Dictionary<int, int> I { get; set; }
            public Dictionary<long, int> L { get; set; }
            public Dictionary<bool, int> B { get; set; }
            public Dictionary<string, int> S { get; set; }
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
            public Guid G { get; set; }
        }

    }
}