using Examples.ReportGeneration.DataObjects;

namespace Examples.ReportGeneration
{
    public class ReportGenerator : IReportGenerator
    {
        public Report Generate(int customerId)
        {
            return new Report("Report for " + customerId);
        }
    }
}