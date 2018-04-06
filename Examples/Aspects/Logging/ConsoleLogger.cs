using System;

namespace Examples.Aspects.Logging
{
    public class ConsoleLogger : ILogger
    {
        public void LogSuccess(LoggingData[] loggingData)
        {
            Console.WriteLine("[Success]");
            foreach (var data in loggingData)
            {
                Console.WriteLine(data.Name + ": " + data.Value);
            }
        }

        public void LogError(LoggingData[] loggingData)
        {
            Console.WriteLine("[Failure]");
            foreach (var data in loggingData)
            {
                Console.WriteLine(data.Name + ": " + data.Value);
            }
        }
    }
}