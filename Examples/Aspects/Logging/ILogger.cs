namespace Examples.Aspects.Logging
{
    public interface ILogger
    {
        void LogSuccess(LoggingData[] loggingData);
        void LogError(LoggingData[] loggingData);
    }
}