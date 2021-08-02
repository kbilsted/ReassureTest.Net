using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace ReassureTest.Tests
{
    public class NonNestedTests
    {
        [Test]
        public void Assert_empty_class_Then_is_empty()
        {
            new EmptyClass().Is("");
        }

        [Test]
        public void Assert_value_on_an_empty_class()
        {
            var ex = Assert.Throws<AssertionException>(() => new EmptyClass().Is("{ }"));
            Assert.AreEqual("Expected: { }\r\nBut was:  <empty>    (all fields have been filtered away)", ex.Message);
        }

        [Test]
        public void SimpleAsserts()
        {
            1.Is("1");
            2L.Is("2");
            2M.Is("2");
            true.Is("true");
            false.Is("false");
            SimpleTypes st = null;
            st.Is("null");
        }

        [Test]
        public void Nested_object_ordering_of_fields_is_unimportant()
        {
            NewSimpleTypes().Is(@"{
                S2 = `hello ""Quotes""`
                Dob = 43
                I = 42
                Float = 44
                L = 42978239382333
                B = true
                D = 2021-06-27T12:13:55
                G = guid-0
                Dec = 45.0
                S = `hello world`
            }");
        }

        [Test]
        public void Int_unequal()
        {
            var val = NewSimpleTypes();
            val.I = 38938;

            var ex = Assert.Throws<AssertionException>(() => val.Is(NewSimpleTypesExpected));
            Assert.AreEqual("Path: 'I'.\r\nExpected: 42\r\nBut was:  38938", ex.Message);
        }

        [Test]
        public void Exceptions_are_mapped_to_simpler_type()
        {
            try
            {
                int i = 0;
                Console.WriteLine(2 / i);
            }
            catch (Exception e)
            {
                e.Is(@"{
                    Message = `Attempted to divide by zero.`
                    Data = [  ]
                    Type = `System.DivideByZeroException`
                } ");
            }
        }

        [Test]
        public void Decimal_unequal()
        {
            var val = NewSimpleTypes();
            val.Dec = 38938.3M;

            var ex = Assert.Throws<AssertionException>(() => val.Is(NewSimpleTypesExpected));
            Assert.AreEqual("Path: 'Dec'.\r\nExpected: 45.0\r\nBut was:  38938.3", ex.Message);
        }

        [Test]
        public void NonNestedObject_printAst()
        {
            string printed = null;
            var cfg = Reassure.DefaultConfiguration.DeepClone();
            cfg.TestFrameworkIntegration.Print = s => printed = s;

            try
            {
                NewSimpleTypes().With(cfg).Is("");
            }
            catch (Exception) { }
            Assert.AreEqual(@"Actual is:
{
    I = 42
    Dob = 43
    Dec = 45.0
    Float = 44
    L = 42978239382333
    B = true
    G = guid-0
    S = `hello world`
    S2 = `hello ""Quotes""`
    D = 2021-06-27T12:13:55
}", printed);
        }

        [Test]
        public void NonNestedObject_printAst_with_exact_guid_values()
        {
            string printed = null;
            var cfg = TestsSetup.ExactGuidValuesCfg;
            cfg.TestFrameworkIntegration.Print = s => printed = s;
            try
            {
                NewSimpleTypes().With(cfg).Is("");
            }
            catch (Exception) { }
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
                G = guid-0
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
                G = guid-0
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
            var ex = Assert.Throws<AssertionException>(() => new SimpleTypesDictionaries
            {
                I = new Dictionary<int, int>() { { 42, 43 } },
            }.Is(@"{    I = [
                    {            Key = 42            Value = 43        },
                    {            Key = 111           Value = 222        }]}"));
            Assert.AreEqual("Path: 'I[1]'.\r\nArray length mismatch. Expected array lengh: 2 but was: 1.", ex.Message);
        }

        [Test]
        public void Dictionary_missing_first_value()
        {
            var ex = Assert.Throws<AssertionException>(() => new SimpleTypesDictionaries
            {
                I = new Dictionary<int, int>() { { 111, 222 } },
            }.Is(@"{    I = [
                    {            Key = 42            Value = 43        },
                    {            Key = 111           Value = 222        }    ]}"));
            Assert.AreEqual("Path: 'I[0].Key'.\r\nExpected: 42\r\nBut was:  111", ex.Message);
        }

        [Test]
        public void Dictionary_with_wrong_non_first_value()
        {
            var simple = new SimpleTypesDictionaries()
            {
                L = new Dictionary<long, int>() { { 4, 222221 } },
            };

            var ex = Assert.Throws<AssertionException>(() =>
            simple.Is(@"{
                       I = ?
                       L = [        {             Key = 4297842978             Value = 1        }    ]}"));

            Assert.AreEqual("Path: 'L[0].Key'.\r\nExpected: 4297842978\r\nBut was:  4", ex.Message);
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

        class SimpleTypesDictionaries
        {
            public Dictionary<int, int> I { get; set; }
            public Dictionary<long, int> L { get; set; }
            public Dictionary<bool, int> B { get; set; }
            public Dictionary<string, int> S { get; set; }
        }

        class EmptyClass
        {
        }
    }
}
