using System;

namespace ReassureTest
{
    public class AssertException : Exception
    {
        public AssertException(string msg) : base(msg)
        {
        }
    }
}