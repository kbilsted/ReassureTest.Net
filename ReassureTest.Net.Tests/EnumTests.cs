using System;
using NUnit.Framework;

namespace ReassureTest.Tests
{
    public class EnumTests
    {
        enum MyEnum
        {
            Good, Evil
        }

        class EnumHolder
        {
            public MyEnum Enum { get; set; }
        }

        [Test]
        public void When_comparing_enum_value_Then_match()
        {
            new EnumHolder { Enum = MyEnum.Evil }.Is(@"{ Enum = Evil }");
        }

        [Test]
        public void When_comparing_enum_value_with_wildcard_Then_match()
        {
            new EnumHolder { Enum = MyEnum.Evil }.Is(@"{ Enum = * }");
            new EnumHolder { Enum = MyEnum.Evil }.Is(@"{ Enum = ? }");
        }

        [Test]
        public void When_comparing_enum_value_with_nonmatch_Then_fail()
        {
            Reassure.Catch(() => new EnumHolder { Enum = MyEnum.Good }.Is(@"{ Enum = Evil }"))
                .Message.Is(@"`Path: 'Enum'.*
Expected: Evil
But was:  Good`");
        }
    }
}