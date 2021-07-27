using NUnit.Framework;

namespace ReassureTest.Tests
{
    public class StringTests
    {
        [Test]
        public void String_tests()
        {
            "null".Is("`null`");

            // for the moment we support simple strings - but this is really never a case in real-life
            "ss".Is("ss");
        }

        [Test]
        public void String_tests_unequal()
        {
            var ex = Assert.Throws<AssertException>(() => "other".Is("`asdf`"));
            Assert.AreEqual("Path: ''.\r\nExpected: \"asdf\"\r\nBut was:  \"other\"", ex.Message);
        }

        [Test]
        public void String_null_compared_to_null()
        {
            string s = null;
            s.Is("null");
        }

        [Test]
        public void String_null_vs_value()
        {
            string s = null;

            var ex = Assert.Throws<AssertException>(() => s.Is("ddd"));

            Assert.AreEqual("Path: ''.\r\nExpected: ddd\r\nBut was:  null", ex.Message);
        }

        [Test]
        public void String_value_vs_null()
        {
            string s = "some string";

            var ex = Assert.Throws<AssertException>(() => s.Is("null"));

            Assert.AreEqual("Path: ''.\r\nExpected: null\r\nBut was:  not null", ex.Message);
        }
    }
}