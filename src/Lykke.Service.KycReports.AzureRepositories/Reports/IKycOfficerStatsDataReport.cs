using System;

namespace Lykke.Service.KycReports.AzureRepositories.Reports
{
    public interface IKycOfficerStatsDataReport
    {
        DateTime ReportDay { get; }
        string KycOfficer { get; }
        string PartnerName { get; }
        int OnBoardedCount { get; }
        int DeclinedCount { get; }
        int ToResubmitCount { get; }

        string RowId { get; }
    }
}
