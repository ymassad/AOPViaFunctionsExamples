namespace Examples.EmailSending.DataObjects
{
    public class EmailBody
    {
        public EmailBody(string value)
        {
            Value = value;
        }

        public string Value { get; }
    }
}