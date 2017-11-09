using System;

namespace Lykke.Service.KycReports.AzureRepositories.Reports
{
    public interface IKycReportDailyLeadership
    {
        DateTime ReportDay { get; }
        int PendingAppsCountAtStart { get; }
        int PendingAppsCountAtEnd { get; }
        int SubmittedAppsCount { get; }
        int ProcessedAppsCount { get; }
        int SpiderCheckDoneAppsCount { get; }
    }
}
