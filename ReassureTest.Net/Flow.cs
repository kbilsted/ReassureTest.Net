namespace ReassureTest
{
    public class Flow
    {
        public readonly bool HasResult;
        public readonly object? Value;

        public static readonly Flow Skip = new Flow(false, null);
        
        public static readonly Flow UseNull = new Flow(true, null);

        public static Flow Use(object? value) => new Flow(true, value);

        /// <summary> Use static methods to create instances </summary>
        private Flow(bool hasResult, object? value)
        {
            HasResult = hasResult;
            Value = value;
        }
    }
}