using System;
using System.Collections.Immutable;
using NUnit.Framework;

namespace ReassureTest.Tests
{
    public class CollectionTests
    {
        [Test]
        public void Empty_array_is_printed_without_spaces()
        {
            Assert.AreEqual("[]", (new int[0].Is("[]")));
            Assert.AreEqual("[]", (new int[0].Is("[ ]")));
            Assert.AreEqual("[]", (new int[0].Is(" [  ] ")));
        }

        [Test]
        public void Simple_array_elements_are_printed_as_one_line()
        {
            Assert.AreEqual("[ 2, 3, 4 ]", (new int[] { 2, 3, 4 }.Is("[2,3,4]")));
            Assert.AreEqual("[ 2, 3, 4 ]", (new int[] { 2, 3, 4 }.Is(@"[ 2,   3,   
4]")));
        }

        [Test]
        public void Default_ImmutableArray_should_not_throw_exception_when_traversed()
        {
            new ImmitableArrayHolder().Is("");
        }

        class ImmitableArrayHolder
        {
            public ImmutableArray<int> arr = default;
        }

        [Test]
        public void Array()
        {
            Reassure.Is(new SimpleTypesArrays()
            {
                I = new[] { 42, 43 },
                I2 = new[] { new[] { 1, 2, 3 }, new[] { 4, 5 } },
                L = new[] { 42978239382333L },
                B = new[] { true, false },
                S = new[] { "hello world" },
            }, @"{
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
            var ex = Assert.Throws<AssertionException>(() => Reassure.Is(new SimpleTypesArrays(), @"{ I = [ 42, 43 ]}"));
            Assert.AreEqual("Path: 'I'.\r\nExpected: not null\r\nBut was: null", ex.Message);
        }

        [Test]
        public void Array_when_expecting_null_values_and_is_null_Then_succeed()
        {
            Reassure.Is(new SimpleTypesArrays(), @"{
    I = null
    I2 = null
    L = null
    B = null
    S = null
}");
        }

        private class SimpleTypesArrays
        {
            public int[] I { get; set; }
            public int[][] I2 { get; set; }
            public long[] L { get; set; }
            public bool[] B { get; set; }
            public string[] S { get; set; }
        }
    }
}