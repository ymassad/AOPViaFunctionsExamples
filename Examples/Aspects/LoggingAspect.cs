using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Examples.Aspects.Logging;
using Functions;

namespace Examples.Aspects
{
    public static class LoggingAspect
    {
        private class Aspect<TInput, TOutput> : IFunction<TInput, TOutput>
        {
            private readonly IFunction<TInput, TOutput> decorated;

            private readonly ILoggingDataExtractor<TInput, TOutput> loggingDataExtractor;

            private readonly LoggingData[] constantLoggingData;

            private readonly ILogger logger;

            public Aspect(
                IFunction<TInput, TOutput> decorated,
                ILogger logger,
                ILoggingDataExtractor<TInput, TOutput> loggingDataExtractor,
                LoggingData[] constantLoggingData)
            {
                this.decorated = decorated;

                this.logger = logger;
                this.loggingDataExtractor = loggingDataExtractor;
                this.constantLoggingData = constantLoggingData;
            }

            public TOutput Invoke(TInput input)
            {
                var preInvocationData = loggingDataExtractor.ExtractFromInput(input);

                TOutput output;

                try
                {
                    output = decorated.Invoke(input);
                }
                catch (Exception e)
                {
                    var errorData = loggingDataExtractor.ExtractFromException(e);

                    logger.LogError(constantLoggingData.Concat(preInvocationData).Concat(errorData).ToArray());

                    throw;
                }

                var outputData = loggingDataExtractor.ExtractFromOutput(output);

                logger.LogSuccess(constantLoggingData.Concat(preInvocationData).Concat(outputData).ToArray());

                return output;
            }
        }

        public static IFunction<TInput, TOutput> ApplyLoggingAspect<TInput, TOutput>(
            this IFunction<TInput, TOutput> decorated,
            string operationName,
            ILogger logger,
            Action<LoggingDataExtractorBuilder<TInput, TOutput>> extractorBuilderAction)
        {
            return ApplyLoggingAspect(
                decorated,
                new[] { new LoggingData("Operation", operationName) },
                logger,
                extractorBuilderAction);
        }

        public static IFunction<TInput, TOutput> ApplyLoggingAspect<TInput, TOutput>(
            this IFunction<TInput, TOutput> decorated,
            LoggingData[] constantLoggingData,
            ILogger logger,
            Action<LoggingDataExtractorBuilder<TInput, TOutput>> extractorBuilderAction)
        {
            var builder = new LoggingDataExtractorBuilder<TInput, TOutput>();

            extractorBuilderAction(builder);

            var extractor = builder.BuildExtractor();

            return new Aspect<TInput, TOutput>(decorated, logger, extractor, constantLoggingData);
        }

        public class LoggingDataExtractorBuilder<TInput, TOutput>
        {
            List<(Func<TInput, string> nameSelector, Func<TInput, object> selector)> inputIncludes =
                new List<(Func<TInput, string> name, Func<TInput, object> selector)>();

            List<(Func<TOutput, string> nameSelector, Func<TOutput, object> selector)> outputIncludes =
                new List<(Func<TOutput, string> nameSelector, Func<TOutput, object> selector)>();

            List<(Func<Exception, string> nameSelector, Func<Exception, object> selector)> errorIncludes =
                new List<(Func<Exception, string> nameSelector, Func<Exception, object> selector)>();

            public LoggingDataExtractorBuilder<TInput, TOutput> IncludeInputs(Expression<Func<TInput, object>> newAnonymousObjectExpression)
            {
                if (!(newAnonymousObjectExpression.Body is NewExpression newExpression))
                    throw new Exception("Expression must create a new anonymous object");

                var constructorParameters = newExpression.Constructor.GetParameters();

                var parametersAndCorrespondingArguments = constructorParameters.Zip(
                    newExpression.Arguments,
                    (param, arg) => new
                    {
                        Parameter = param,
                        Argument = arg
                    }).ToList();

                if (!parametersAndCorrespondingArguments.All(x => x.Argument is MemberExpression))
                    throw new Exception("Anonymous object items must be input access expressions");

                var namesAndValueGettingFunctions =
                    parametersAndCorrespondingArguments
                        .Select(
                            x =>
                                new
                                {
                                    Name = x.Parameter.Name,
                                    AccessFunc =
                                    Expression.Lambda<Func<TInput, object>>(
                                            x.Argument,
                                            newAnonymousObjectExpression.Parameters)
                                        .Compile()
                                }).ToList();

                foreach (var item in namesAndValueGettingFunctions)
                    inputIncludes.Add((_ => item.Name, item.AccessFunc));

                return this;
            }

            public LoggingDataExtractorBuilder<TInput, TOutput> IncludeInput(Func<TInput, string> nameSelector, Func<TInput, object> selector)
            {
                inputIncludes.Add((nameSelector, selector));

                return this;
            }

            public LoggingDataExtractorBuilder<TInput, TOutput> IncludeOutput(Func<TOutput, string> nameSelector, Func<TOutput, object> selector)
            {
                outputIncludes.Add((nameSelector, selector));

                return this;
            }

            public LoggingDataExtractorBuilder<TInput, TOutput> IncludeOutput(Func<TOutput, object> selector)
            {
                outputIncludes.Add((_ => "Return value", selector));

                return this;
            }

            public LoggingDataExtractorBuilder<TInput, TOutput> IncludeOutput()
            {
                outputIncludes.Add((_ => "Return value", x => x));

                return this;
            }

            public LoggingDataExtractorBuilder<TInput, TOutput> IncludeOutput(Func<TOutput, string> nameSelector)
            {
                outputIncludes.Add((nameSelector, x => x));

                return this;
            }

            public LoggingDataExtractorBuilder<TInput, TOutput> IncludeError(Func<Exception, string> nameSelector, Func<Exception, object> selector)
            {
                errorIncludes.Add((nameSelector, selector));

                return this;
            }

            public LoggingDataExtractorBuilder<TInput, TOutput> IncludeErrorMessage()
            {
                errorIncludes.Add((_ => "Error message", x => x.Message));

                return this;
            }

            public LoggingDataExtractorBuilder<TInput, TOutput> IncludeErrorStackTrace()
            {
                errorIncludes.Add((_ => "Stack trace", x => x.StackTrace));

                return this;
            }

            public ILoggingDataExtractor<TInput, TOutput> BuildExtractor()
            {
                return new Extractor(inputIncludes, outputIncludes, errorIncludes);
            }

            private class Extractor : ILoggingDataExtractor<TInput, TOutput>
            {
                private readonly List<(Func<TInput, string> nameSelector, Func<TInput, object> selector)> inputIncludes;

                private readonly List<(Func<TOutput, string> nameSelector, Func<TOutput, object> selector)> outputIncludes;

                private readonly List<(Func<Exception, string> nameSelector, Func<Exception, object> selector)> errorIncludes;

                public Extractor(
                    List<(Func<TInput, string> nameSelector, Func<TInput, object> selector)> inputIncludes,
                    List<(Func<TOutput, string> nameSelector, Func<TOutput, object> selector)> outputIncludes,
                    List<(Func<Exception, string> nameSelector, Func<Exception, object> selector)> errorIncludes)
                {
                    this.inputIncludes = inputIncludes;
                    this.outputIncludes = outputIncludes;
                    this.errorIncludes = errorIncludes;
                }

                public LoggingData[] ExtractFromInput(TInput input)
                {
                    return inputIncludes.Select(x => new LoggingData(x.nameSelector(input), x.selector(input))).ToArray();
                }

                public LoggingData[] ExtractFromOutput(TOutput output)
                {
                    return outputIncludes.Select(x => new LoggingData(x.nameSelector(output), x.selector(output))).ToArray();
                }

                public LoggingData[] ExtractFromException(Exception exception)
                {
                    return errorIncludes.Select(x => new LoggingData(x.nameSelector(exception), x.selector(exception))).ToArray();
                }
            }
        }

        public interface ILoggingDataExtractor<in TInput, in TOutput>
        {
            LoggingData[] ExtractFromInput(TInput input);

            LoggingData[] ExtractFromOutput(TOutput output);

            LoggingData[] ExtractFromException(Exception exception);
        }
    }
}