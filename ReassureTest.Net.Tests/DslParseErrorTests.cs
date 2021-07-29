using System;
using NUnit.Framework;

namespace ReassureTest.Tests
{
    public class DslParseErrorTests
    {
        [Test]
        public void Missing_value_long_string_is_trimmed_at_end()
        {
            var ex = Assert.Throws<Exception>(() => "".Is("{ a =                            }                                  "));
            Assert.AreEqual("Expecting either '{', '[', or a value. Can not accept '}' at position: 33 (of token kind 'Meta' at token number: '3')\r\n{ a =                            }                                ...", ex.Message);
        }

        [Test]
        public void Missing_value_long_string_is_trimmed_at_both_ends()
        {
            var ex = Assert.Throws<Exception>(() => "".Is("                                 { a =                            }                                  "));
            Assert.AreEqual("Expecting either '{', '[', or a value. Can not accept '}' at position: 66 (of token kind 'Meta' at token number: '3')\r\n...{ a =                            }                                ...", ex.Message);
        }

        [Test]
        public void Missing_value_short_string_is_printed()
        {
            var ex = Assert.Throws<Exception>(() => "".Is("{a = } "));
            Assert.AreEqual("Expecting either '{', '[', or a value. Can not accept '}' at position: 5 (of token kind 'Meta' at token number: '3')\r\n{a = } ", ex.Message);
        }
    }
}
