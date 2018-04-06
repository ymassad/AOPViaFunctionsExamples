using System;
using System.Threading;
using Functions;

namespace Examples.Aspects
{
    public static class RetryAspect
    {
        public static IFunction<TInput, TOutput> ApplyRetryAspect<TInput, TOutput>(
            this IFunction<TInput, TOutput> function,
            int numberOfTimesToRetry,
            TimeSpan waitTimeBeforeRetries)
        {
            return new Decorator<TInput, TOutput>(function, numberOfTimesToRetry, waitTimeBeforeRetries);
        }

        private class Decorator<TInput, TOutput> : IFunction<TInput, TOutput>
        {
            private readonly IFunction<TInput, TOutput> decorated;

            private readonly int numberOfTimesToRetry;

            private readonly TimeSpan waitTimeBeforeRetries;

            public Decorator(IFunction<TInput, TOutput> decorated, int numberOfTimesToRetry, TimeSpan waitTimeBeforeRetries)
            {
                this.decorated = decorated;
                this.numberOfTimesToRetry = numberOfTimesToRetry;
                this.waitTimeBeforeRetries = waitTimeBeforeRetries;
            }

            public TOutput Invoke(TInput input)
            {
                int timesRetried = 0;

                while (true)
                {
                    try
                    {
                        return decorated.Invoke(input);
                    }
                    catch
                    {
                        timesRetried++;

                        if(timesRetried == numberOfTimesToRetry)
                            throw;

                        Thread.Sleep(waitTimeBeforeRetries);
                    }
                }
            }
        }
    }
}