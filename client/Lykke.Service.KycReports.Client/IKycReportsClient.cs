
using System;
using System.Threading.Tasks;

namespace Lykke.Service.KycReports.Client
{
    public interface IKycReportsClient
    {
        Task<string> GetKycOfficerStatsJsonAsync(DateTime dateFrom, DateTime dateTo);
        Task<string> GetKycOfficersPerformanceJsonAsync(DateTime dateFrom, DateTime dateTo);
        Task<string> GetKycReportDailyLeadershipDataJsonAsync(DateTime dateFrom, DateTime dateTo);
        Task<bool?> RebuildKycOfficerStats();
        Task<bool?> RebuildKycOfficersPerformance();
    }
}
