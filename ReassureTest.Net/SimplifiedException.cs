using System;
using System.Collections;

namespace ReassureTest
{
    public class SimplifiedException
    {
        public string Message { get; set; }
        public IDictionary Data { get; set; }
        public string Type { get; set; }

        public SimplifiedException(Exception e)
        {
            Message = e.Message;
            Data = e.Data;
            Type = e.GetType().ToString();
        }
    }
}