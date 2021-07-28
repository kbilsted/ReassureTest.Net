using System;
using NUnit.Framework;

namespace ReassureTest.Tests
{
    public class ExceptionTests
    {
        [Test]
        public void Exceptions_are_transformed_to_simple_form()
        {
            var ex = new Exception("message") {Data = {{"a", "b"}, {"1", "2"}}};
            ex.Is(@"{
                Message = `message`
                Data = [
                    {
                        Key = `a`
                        Value = `b`
                    },
                    {
                        Key = `1`
                        Value = `2`
                    }
                ]
                Type = `System.Exception`
            }");
        }
    }
}