using System;

namespace ReassureTest.Net
{
    public class AssertException : Exception
    {
        public AssertException(string msg) : base(msg)
        {
        }
    }
}