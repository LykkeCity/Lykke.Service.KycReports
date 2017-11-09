using System;
using System.Threading.Tasks;

namespace Lykke.Service.KycReports.Core.Domain.Reports
{
    public interface IKycReportingService {
        Task<string> GetKycOfficerStatsJsonAsync(DateTime dateFrom, DateTime dateTo);
        Task<string> GetKycOfficersPerformanceJsonAsync(DateTime dateFrom, DateTime dateTo);
        Task<string> GetKycReportDailyLeadershipDataJsonAsync(DateTime dateFrom, DateTime dateTo);

        Task<bool> RebuildKycOfficerStats();
        Task<bool> RebuildKycOfficersPerformance();
    }
}
