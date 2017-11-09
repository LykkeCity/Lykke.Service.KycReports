using System;
using System.Collections.Generic;
using System.Linq;

namespace Lykke.Service.KycReports.AzureRepositories.Reports
{
    public class KycReportDailyLeadership : IKycReportDailyLeadership
    {
        public DateTime ReportDay { get; set; }
        public int PendingAppsCountAtStart { get; set; }
        public int PendingAppsCountAtEnd { get; set; }
        public int SubmittedAppsCount { get; set; }
        public int ProcessedAppsCount { get; set; }
        public int SpiderCheckDoneAppsCount { get; set; }

        public KycReportDailyLeadership()
        {
            PendingAppsCountAtStart = -1;
            PendingAppsCountAtEnd = -1;
            SubmittedAppsCount = -1;
            ProcessedAppsCount = -1;
            SpiderCheckDoneAppsCount = -1;
        }

        public static IEnumerable<string> GenerateRowids(DateTime from, DateTime to)
        {
            var startDate = new DateTime(from.Year, from.Month, from.Day);
            var endDate =  new DateTime(to.Year, to.Month, to.Day);

            var datesArray = Enumerable.Range(0, 1 + endDate.Subtract(startDate).Days)
                .Select(offset => startDate.AddDays(offset))
                .ToArray();

            return datesArray.Select(i => i.Ticks.ToString());
        }
    }
}
