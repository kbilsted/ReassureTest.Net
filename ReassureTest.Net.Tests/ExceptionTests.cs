using System;
using NUnit.Framework;

namespace ReassureTest.Tests
{
    public class ExceptionTests
    {
        [Test]
        public void Exceptions_are_mapped_to_simpler_type_without_data()
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
                    Type = `System.DivideByZeroException`
                } ");
            }
        }

        [Test]
        public void Exceptions_are_transformed_to_simpler_type_with_data()
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