using NUnit.Framework;

namespace ReassureTest.Tests
{
    public class StringTests
    {
        [Test]
        public void String_match()
        {
            "null".Is("`null`");

            // for the moment we support simple strings - but this is really never a case in real-life
            "ss".Is("ss");
        }

        [Test]
        [TestCase("`*`", Reason = "all")]
        [TestCase("`* text`", Reason = "start")]
        [TestCase("`some *`", Reason = "end")]
        [TestCase("`*me te*`", Reason = "middle")]
        public void String_wildcard_match(string expected)
        {
            "some text".Is(expected);
        }

        [Test]
        public void String_tests_unequal()
        {
            var ex = Assert.Throws<AssertionException>(() => "other".Is("`asdf`"));
            Assert.AreEqual("Expected: \"asdf\"\r\nBut was:  \"other\"", ex.Message);
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

            var ex = Assert.Throws<AssertionException>(() => s.Is("`ddd`"));

            Assert.AreEqual("Expected: \"ddd\"\r\nBut was:  null", ex.Message);
        }

        [Test]
        public void String_value_vs_null()
        {
            string s = "some string";

            var ex = Assert.Throws<AssertionException>(() => s.Is("null"));

            Assert.AreEqual("Expected: null\r\nBut was:  \"some string\"", ex.Message);
        }
    }
}