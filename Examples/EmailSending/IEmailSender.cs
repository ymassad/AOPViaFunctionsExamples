using System;
using Examples.EmailSending.DataObjects;
using Functions;

namespace Examples.EmailSending
{
    public interface IEmailSender
    {
        void SendEmail(EmailAddress from, EmailAddress to, string subject, EmailBody body);
    }

    public static class EmailSenderMapExtensions
    {
        public static IEmailSender Map(this IEmailSender instance, Func<IFunction<(EmailAddress from, EmailAddress to, string subject, EmailBody body), Unit>, IFunction<(EmailAddress from, EmailAddress to, string subject, EmailBody body), Unit>> decorationFunction)
        {
            return new FromFunctionClass(decorationFunction(new ToFunctionClass(instance)));
        }

        public static IFunction<(EmailAddress from, EmailAddress to, string subject, EmailBody body), Unit> ToFunction(this IEmailSender instance)
        {
            return new ToFunctionClass(instance);
        }

        public static IEmailSender ToEmailSender(this IFunction<(EmailAddress from, EmailAddress to, string subject, EmailBody body), Unit> function)
        {
            return new FromFunctionClass(function);
        }

        private class FromFunctionClass : IEmailSender
        {
            private readonly IFunction<(EmailAddress from, EmailAddress to, string subject, EmailBody body), Unit> function;
            public FromFunctionClass(IFunction<(EmailAddress from, EmailAddress to, string subject, EmailBody body), Unit> function)
            {
                this.function = function;
            }

            public void SendEmail(EmailAddress from, EmailAddress to, string subject, EmailBody body)
            {
                this.function.Invoke((from, to, subject, body));
            }
        }

        private class ToFunctionClass : IFunction<(EmailAddress from, EmailAddress to, string subject, EmailBody body), Unit>
        {
            private readonly IEmailSender instance;
            public ToFunctionClass(IEmailSender instance)
            {
                this.instance = instance;
            }

            public Unit Invoke((EmailAddress from, EmailAddress to, string subject, EmailBody body) input)
            {
                this.instance.SendEmail(input.from, input.to, input.subject, input.body);
                return Unit.Default;
            }
        }
    }
}