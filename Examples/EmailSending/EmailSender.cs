using System;
using Examples.EmailSending.DataObjects;

namespace Examples.EmailSending
{
    public class EmailSender : IEmailSender
    {
        public void SendEmail(EmailAddress from, EmailAddress to, string subject, EmailBody body)
        {
            Console.WriteLine("Sending email");
            Console.WriteLine("From: "  + from.Value);
            Console.WriteLine("To: " + to.Value);
            Console.WriteLine("Subject: " + subject);
            Console.WriteLine("Body: " + body.Value);
        }
    }
}