using System;
using Examples.ReportGeneration.DataObjects;
using Functions;

namespace Examples.ReportGeneration
{
    public interface IReportGenerator
    {
        Report Generate(int customerId);
    }

    public static class ReportGeneratorMapExtensions
    {
        public static IReportGenerator Map(this IReportGenerator instance, Func<IFunction<int, Report>, IFunction<int, Report>> decorationFunction)
        {
            return new FromFunctionClass(decorationFunction(new ToFunctionClass(instance)));
        }

        public static IFunction<int, Report> ToFunction(this IReportGenerator instance)
        {
            return new ToFunctionClass(instance);
        }

        public static IReportGenerator ToReportGenerator(this IFunction<int, Report> function)
        {
            return new FromFunctionClass(function);
        }

        private class FromFunctionClass : IReportGenerator
        {
            private readonly IFunction<int, Report> function;
            public FromFunctionClass(IFunction<int, Report> function)
            {
                this.function = function;
            }

            public Report Generate(int customerId)
            {
                return this.function.Invoke(customerId);
            }
        }

        private class ToFunctionClass : IFunction<int, Report>
        {
            private readonly IReportGenerator instance;
            public ToFunctionClass(IReportGenerator instance)
            {
                this.instance = instance;
            }

            public Report Invoke(int input)
            {
                return this.instance.Generate(input);
            }
        }
    }
}