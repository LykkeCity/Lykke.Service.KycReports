
using Lykke.Service.KycReports.AutorestClient.Models;
using System;
using System.Collections.Generic;
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
        Task<IList<KycClientStatRow>> GetKycClientStatsData(DateTime dateFrom, DateTime dateTo);
        Task<IList<KycClientStatRow>> GetKycClientStatsDataShort(DateTime dateFrom, DateTime dateTo);
    }
}
