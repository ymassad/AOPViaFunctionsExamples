using System;
using Examples.Aspects;
using Examples.Aspects.Logging;
using Examples.EmailSending;
using Examples.EmailSending.DataObjects;
using Examples.FileSystem;
using Examples.FileSystem.Authorizers;
using Examples.ReportGeneration;

namespace Examples
{
    class Program
    {
        static void Main(string[] args)
        {
            var emailSender =
                new EmailSender()
                    .Map(x => x
                        .ApplyRetryAspect(
                            numberOfTimesToRetry: 2,
                            waitTimeBeforeRetries: TimeSpan.FromSeconds(5)));

            var reportGenerator =
                new ReportGenerator()
                    .Map(x => x
                        .ApplyRetryAspect(
                            numberOfTimesToRetry: 3,
                            waitTimeBeforeRetries: TimeSpan.FromSeconds(10)));

            var fileSystem =
                new FileSystemService()
                    .Map(methods => (
                        methods
                            .getFileContent
                            .ApplyAuthorizationAspect(new FileReadAuthorizer(), fileId => fileId),
                        methods
                            .getFolderContents
                            .ApplyAuthorizationAspect(new FolderReadAuthorizer(), folderId => folderId),
                        methods
                            .copyFileToFolder
                            .ApplyAuthorizationAspect(new FolderWriteAuthorizer(), x => x.folderId)
                            .ApplyAuthorizationAspect(new FileReadAuthorizer(), x => x.fileId)));

            var emailSenderWithLogging =
                new EmailSender()
                    .Map(x => x.ApplyLoggingAspect(
                        "Sending an email",
                        new ConsoleLogger(),
                        builder => builder
                            .IncludeInputs(input => new {From = input.from.Value, To = input.to.Value, input.subject})
                            .IncludeErrorMessage()
                            .IncludeErrorStackTrace()));

            //Test sending an email
            emailSenderWithLogging.SendEmail(
                new EmailAddress("me@me.lab"),
                new EmailAddress("her@her.lab"),
                "Hello",
                new EmailBody("What's up?"));
        }
    }
}
