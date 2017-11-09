using Lykke.Service.KycReports.Core.Domain.Reports;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Lykke.Service.KycReports.AzureRepositories.Reports
{
    public interface IKycReportsRepository
    {
        Task InsertRow(IKycReportDailyLeadership row);
        Task InsertRow(IKycOfficerStatsDataReport row);
        Task InsertRow(IKycOfficersPerformanceRow row);

        Task<IEnumerable<T>> GetRows<T>(IEnumerable<string> rowKeys = null);
        Task<IEnumerable<string>> GetJsonData(KycReportType reportType, bool isDescending = false, IEnumerable<string> rowKeys = null);

        Task<List<string>> GetKycOfficerStatsJsonRows(DateTime from, DateTime to);
        Task<List<string>> GetKycOfficersPerformanceJsonRows(DateTime from, DateTime to);
    }
}
