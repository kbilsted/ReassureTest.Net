using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
            new SimpleTypes()
            {
                I = 42,
                L = 42978239382333L,
                B = true,
                S = "hello world",
                S2 = "hello \"Quotes\""
            }.Is("");
        }

        [Test]
        public void NonNestedObject_with_array()
        {
            new SimpleTypesArrays()
            {
                I = new[]{42,43},
                L = new[]{42978239382333L},
                B = new[]{true},
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
        public void METHOD()
        {
            object a = new
            {
                b = "s",
                c = new[] { new { s = 2 }, new { s = 3 } }
            };

            mymethod(()=>a);
        }

        static void mymethod(Expression<Func<object>> o)
        {
            Console.WriteLine(o.Body.GetType());
            Console.WriteLine("base:"+o.Body.GetType().BaseType);
            MemberExpression m = (MemberExpression) o.Body;
            
            Console.WriteLine("member::"+string.Join(",", m.Member.GetType().GetProperties().Select(x => x.Name + ":::" + x.PropertyType)));
            Console.WriteLine(string.Join(",", o.Body.GetType().GetProperties().Select(x => x.Name + ":::" + x.PropertyType)));
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