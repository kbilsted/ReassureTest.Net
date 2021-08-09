using System;
using NUnit.Framework;

namespace ReassureTest.Tests
{
    class ThrowsTests
    {
        [Test]
        public void Inline_action()
        {
            Reassure.Catch(() => throw new Exception("eee")).Is(@"{
                Message = `eee`
                Type = `System.Exception`
            }");

            Reassure.With(Reassure.DefaultConfiguration).Catch(() => throw new Exception("eee")).Is(@"{
                Message = `eee`
                Type = `System.Exception`
            }");

            Reassure.With(Reassure.DefaultConfiguration).Catch(() =>
            {
                var exception = new Exception("eee");
                exception.Data.Add("a", "b");
                throw exception;
            }).Is(@"{
                Message = `eee`
                Data = [ { Key = `a` Value = `b` } ]
                Type = `System.Exception`
            }");
        }

        [Test]
        public void Inline_func()
        {
            Reassure.Catch(() => ((string) null).Length).Is(@"{
                Message = `Object reference not set to an instance of an object.`
                Type = `System.NullReferenceException`
            }");

            Reassure.With(Reassure.DefaultConfiguration).Catch(() => ((string) null).Length).Is(@"{
                Message = `Object reference not set to an instance of an object.`
                Type = `System.NullReferenceException`
            }");
        }
    }
}