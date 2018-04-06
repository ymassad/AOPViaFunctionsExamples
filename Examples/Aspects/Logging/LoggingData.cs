namespace Examples.Aspects.Logging
{
    public class LoggingData
    {
        public LoggingData(string name, object value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; }

        public object Value { get; }
    }
}
