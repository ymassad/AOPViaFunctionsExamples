namespace Examples.EmailSending.DataObjects
{
    public class EmailAddress
    {
        public EmailAddress(string value)
        {
            Value = value;
        }

        public string Value { get; }
    }
}