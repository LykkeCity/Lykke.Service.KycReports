using Lykke.Service.KycReports.Core.Domain.Reports;
using System;

namespace Lykke.Service.KycReports.AzureRepositories.Reports
{
    public interface IKycOfficersPerformanceRow
    {
        DateTime ReportDay { get; }
        string KycOfficer { get; }
        KycOfficerReportOperationType Operation { get; }
        string ClientEmail { get; }

        string RowId { get; }
    }
}
