using System;
using NUnit.Framework;

namespace ReassureTest.Tests
{
    public class DslParseErrorTests
    {
        [Test]
        public void Missing_value_long_string_is_trimmed_at_end()
        {
            Assert.AreEqual("Parse error. Expecting either '{', '[', or a value. Can not accept '}' at position: 33 (of token kind 'Meta' at token number: '3')\r\n{ a =                            }                                ...",
                GetThrows(() => "".Is("{ a =                            }                                  ")));
        }

        [Test]
        public void Missing_value_long_string_is_trimmed_at_both_ends()
        {
            Assert.AreEqual("Parse error. Expecting either '{', '[', or a value. Can not accept '}' at position: 66 (of token kind 'Meta' at token number: '3')\r\n...{ a =                            }                                ...",
                GetThrows(() => "".Is("                                 { a =                            }                                  ")));
        }

        [Test]
        public void Missing_value_short_string_is_printed()
        {
            Assert.AreEqual("Parse error. Expecting either '{', '[', or a value. Can not accept '}' at position: 5 (of token kind 'Meta' at token number: '3')\r\n{a = } ",
                GetThrows(() => "".Is("{a = } ")));
        }

        [Test]
        public void Missing_equal_should_fail()
        {
            Assert.AreEqual("Parse error. Expected '=', but got '2' position: 4 (of kind 'Value' at token: 2)\r\n{ a 2 }",
                GetThrows(() => "".Is("{ a 2 }")));
        }

        [Test]
        public void Missing_fieldname_as_meta_character_open_complex_should_fail()
        {
            Assert.AreEqual("Parse error. Expected a fieldname, but got '{' position: 2 (of kind 'Meta' at token: 1)\r\n{ { = 2 }",
                GetThrows(() => "".Is("{ { = 2 }")));
        }

        [Test]
        public void Missing_fieldname_as_meta_character_eql_should_fail()
        {
            Assert.AreEqual("Parse error. Expected a fieldname, but got '=' position: 2 (of kind 'Meta' at token: 1)\r\n{ = 2 }",
                GetThrows(() => "".Is("{ = 2 }")));
        }

        public static string GetThrows(Action a)
        {
            var ex = Assert.Throws<InvalidOperationException>(() => a());
            return ex.Message;
        }
    }
}
