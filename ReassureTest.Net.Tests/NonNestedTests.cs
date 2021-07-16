using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace ReassureTest.Net.Tests
{
    public class NonNestedTests
    {
        [Test]
        public void SimpleAsserts()
        {
            Guid g = Guid.NewGuid();
            g.Is(g.ToString());

            1.Is("1");
            2L.Is("2");
            2M.Is("2");
            true.Is("true");
            false.Is("false");
            SimpleTypes st = null;
            st.Is("null");
        }

        [Test]
        public void string_tests()
        {
            "null".Is("`null`");

            // for the moment we support simple strings - but this is really never a case in real-life
            "ss".Is("ss");
        }

        [Test]
        public void string_null_compared_to_null()
        {
            string s = null;
            s.Is("null");
        }

        [Test]
        public void string_null_compared_to_value()
        {
            string s = null;

            var ex = Assert.Throws<AssertException>(() => s.Is("ddd"));

            Assert.IsTrue(ex.Message.StartsWith("Path: ''. Expected: \"ddd\"\r\n  But was:  null"));
        }

        [Test]
        public void DateTimeTests_now()
        {
            DateTime.Now.Is("now");
            DateTime.Now.AddSeconds(-1.0).Is("now");
            DateTime.Now.AddSeconds(1.0).Is("now");

            var ex = Assert.Throws<AssertException>(() => DateTime.Now.AddSeconds(10.0).Is("now"));
            Assert.IsTrue(ex.Message.StartsWith(@"Path: ''. Expected: 20"));
        }

        [Test]
        public void DateTimeTests_dates()
        {
            var time = new DateTime(1999, 9, 19, 12, 54, 02);
            time.Is("1999-09-19T12:54:02");
            time.Is("1999-09-19T12:54:01");
            time.Is("1999-09-19T12:54:03");

            var ex = Assert.Throws<AssertException>(() => time.AddSeconds(10.0).Is("1999-09-19T12:54:02"));
            Assert.AreEqual("Path: ''. Expected: 1999-09-19 12:54:02\r\n  But was:  1999-09-19 12:54:12\r\n", ex.Message);
        }

        [Test]
        public void DateTimeTests_null()
        {
            DateTime? t = null;

            var ex = Assert.Throws<AssertException>(() => t.Is("1999-09-19T12:54:02"));

            Assert.AreEqual("Path: ''. Expected: not null\r\nBut was: null", ex.Message);
        }

        [Test]
        public void Wrong_int()
        {
            var val = NewSimpleTypes();
            val.I = 38938;

            var ex = Assert.Throws<AssertException>(() => val.Is(NewSimpleTypesExpected));
            Assert.AreEqual("Path: 'I'. Expected: 42\r\n  But was:  38938\r\n", ex.Message);
        }

        [Test]
        public void NonNestedObject_printAst()
        {
            string printed = null;
            new ReassureTestTester().Is(NewSimpleTypes(), "", s => printed = s, (a, b) => { });
            Assert.AreEqual(@"Actual is:
{
    I = 42
    Dob = 43
    Dec = 45.0
    Float = 44
    L = 42978239382333
    B = true
    G = 123e4567-e89b-12d3-a456-426614174000
    S = `hello world`
    S2 = `hello ""Quotes""`
    D = 2021-06-27T12:13:55
}", printed);
        }

        [Test]
        public void NonNestedObject()
        {
            var simpleTypes = NewSimpleTypes();
            simpleTypes.Is(NewSimpleTypesExpected);
        }

        [Test]
        public void NonNestedObjectNullable()
        {
            var simpleTypes = NewSimpleTypesNullable();
            simpleTypes.Is(NewSimpleTypesExpected);
        }

        [Test]
        public void NonNestedObject_with_null_fields()
        {
            new SimpleTypes().Is(@"{
    I = 0
    Dob = 0
    Dec = 0
    Float = 0
    L = 0
    B = false
    G = 00000000-0000-0000-0000-000000000000
    S = null
    S2 = null
    D = 0001-01-01T00:00:00
}");
        }


        [Test]
        public void NonNestedObject_nullable_with_null_fields()
        {
            new SimpleTypesNullable().Is(@"{
    I = null
    Dob = null
    Dec = null
    Float = null
    L = null
    B = null
    G = null
    S = null
    S2 = null
    D = null
}");
        }


        private const string NewSimpleTypesExpected = @"
{
    I = 42
    Dob = 43
    Dec = 45.0
    Float = 44
    L = 42978239382333
    B = True
    G = 123e4567-e89b-12d3-a456-426614174000
    S = `hello world`
    S2 = `hello ""Quotes""`
    D = 2021-06-27T12:13:55
}";

        private static SimpleTypes NewSimpleTypes() =>
            new SimpleTypes()
            {
                I = 42,
                Dob = 43.0,
                Float = 44.0f,
                Dec = 45.0M,
                L = 42978239382333L,
                B = true,
                G = Guid.Parse("123e4567-e89b-12d3-a456-426614174000"),
                S = "hello world",
                S2 = "hello \"Quotes\"",
                D = new DateTime(2021, 6, 27, 12, 13, 55),
            };
        private static SimpleTypesNullable NewSimpleTypesNullable() =>
            new SimpleTypesNullable()
            {
                I = 42,
                Dob = 43.0,
                Float = 44.0f,
                Dec = 45.0M,
                L = 42978239382333L,
                B = true,
                G = Guid.Parse("123e4567-e89b-12d3-a456-426614174000"),
                S = "hello world",
                S2 = "hello \"Quotes\"",
                D = new DateTime(2021, 6, 27, 12, 13, 55),
            };

        [Test]
        public void Array()
        {
            new SimpleTypesArrays()
            {
                I = new[] { 42, 43 },
                I2 = new[] { new[] { 1, 2, 3 }, new[] { 4, 5 } },
                L = new[] { 42978239382333L },
                B = new[] { true, false },
                S = new[] { "hello world" },
            }.Is(@"{
    I = [ 42, 43 ]
    I2 = [
        [ 1, 2, 3 ],
        [ 4, 5 ]
    ]
    L = [ 42978239382333 ]
    B = [ True, False ]
    S = [ `hello world` ]
}");
        }

        [Test]
        public void Array_when_expecting_values_and_is_null_Then_fail()
        {
            var ex = Assert.Throws<AssertException>(() => new SimpleTypesArrays().Is(@"{ I = [ 42, 43 ]}"));
            Assert.AreEqual("Path: 'I'. Expected: not null\r\nBut was: null", ex.Message);
        }

        [Test]
        public void Array_when_expecting_null_values_and_is_null_Then_succeed()
        {
            new SimpleTypesArrays().Is(@"{
    I = null
    I2 = null
    L = null
    B = null
    S = null
}");
        }

        [Test]
        public void Dictionary()
        {
            new SimpleTypesDictionaries()
            {
                I = new Dictionary<int, int>() { { 42, 43 }, { 111, 222 } },
                L = new Dictionary<long, int>() { { 42978239382333L, -1 } },
                B = new Dictionary<bool, int>() { { true, 2 } },
                S = new Dictionary<string, int>() { { "hello world", 3 } },
            }.Is(@"{
    I = [
        {
            Key = 42
            Value = 43
        },
        {
            Key = 111
            Value = 222
        }
    ]
    L = [
        {
            Key = 42978239382333
            Value = -1
        }
    ]
    B = [
        {
            Key = True
            Value = 2
        }
    ]
    S = [
        {
            Key = `hello world`
            Value = 3
        }
    ]
}");
        }

        [Test]
        public void Dictionary_missing_second_value()
        {
            var ex = Assert.Throws<AssertException>(() => new SimpleTypesDictionaries
            {
                I = new Dictionary<int, int>() { { 42, 43 } },
            }.Is(@"{    I = [
                    {            Key = 42            Value = 43        },
                    {            Key = 111           Value = 222        }]}"));
            Assert.AreEqual("Path: 'I[1]'. Array length mismatch. Expected array lengh: 2 but was: 1.", ex.Message);
        }

        [Test]
        public void Dictionary_missing_first_value()
        {
            var ex = Assert.Throws<AssertException>(() => new SimpleTypesDictionaries
            {
                I = new Dictionary<int, int>() { { 111, 222 } },
            }.Is(@"{    I = [
                    {            Key = 42            Value = 43        },
                    {            Key = 111           Value = 222        }    ]}"));
            Assert.AreEqual("Path: 'I[0].Key'. Expected: 42\r\n  But was:  111\r\n", ex.Message);
        }

        [Test]
        public void Dictionary_with_wrong_non_first_value()
        {
            var simple = new SimpleTypesDictionaries()
            {
                L = new Dictionary<long, int>() { { 4, 222221 } },
            };

            var ex = Assert.Throws<AssertException>(() =>
            simple.Is(@"{
                       I = ?
                       L = [        {             Key = 4297842978             Value = 1        }    ]}"));

            Assert.AreEqual("Path: 'L[0].Key'. Expected: 4297842978\r\n  But was:  4\r\n", ex.Message);
        }

        class SimpleTypes
        {
            public int I { get; set; }
            public double Dob { get; set; }
            public decimal Dec { get; set; }
            public float Float { get; set; }
            public long L { get; set; }
            public bool B { get; set; }
            public Guid G { get; set; }
            public string S { get; set; }
            public string S2 { get; set; }
            public DateTime D { get; set; }
        }

        class SimpleTypesNullable
        {
            public int? I { get; set; }
            public double? Dob { get; set; }
            public decimal? Dec { get; set; }
            public float? Float { get; set; }
            public long? L { get; set; }
            public bool? B { get; set; }
            public Guid? G { get; set; }
            public string S { get; set; }
            public string S2 { get; set; }
            public DateTime? D { get; set; }
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
    }
}