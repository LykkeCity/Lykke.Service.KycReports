using System;
using Common.Log;
using Lykke.Service.KycReports.AutorestClient;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;
using Lykke.Service.KycReports.AutorestClient.Models;
using Lykke.Common.Log;

namespace Lykke.Service.KycReports.Client
{
    public class KycReportsClient : IKycReportsClient
    {
        private readonly ILog _log;
        private readonly IKycReportsAPI _api;

        [Obsolete("Please, use the overload which consumes ILogFactory instead.")]
        public KycReportsClient(string serviceUrl, ILog log)
        {
            _log = log;
            _api = new KycReportsAPI(new Uri(serviceUrl), new HttpClient());
        }

        public KycReportsClient(string serviceUrl, ILogFactory logFactory)
        {
            _log = logFactory.CreateLog(this);
            _api = new KycReportsAPI(new Uri(serviceUrl), new HttpClient());
        }

        public async Task<string> GetKycOfficerStatsJsonAsync(DateTime dateFrom, DateTime dateTo)
        {
            return await _api.ApiKycReportingStatsByDateFromByDateToGetAsync(dateFrom, dateTo);
        }

        public async Task<string> GetKycOfficersPerformanceJsonAsync(DateTime dateFrom, DateTime dateTo)
        {
            return await _api.ApiKycReportingPerformByDateFromByDateToGetAsync(dateFrom, dateTo);
        }

        public async Task<string> GetKycReportDailyLeadershipDataJsonAsync(DateTime dateFrom, DateTime dateTo)
        {
            return await _api.ApiKycReportingLeadershipByDateFromByDateToGetAsync(dateFrom, dateTo);
        }

        public async Task<bool?> RebuildKycOfficerStats()
        {
            return await _api.ApiKycReportingRebuildStatsPostAsync();
        }

        public async Task<bool?> RebuildKycOfficersPerformance()
        {
            return await _api.ApiKycReportingRebuildPerformPostAsync();
        }

        public async Task<IList<KycClientStatRow>> GetKycClientStatsData(DateTime dateFrom, DateTime dateTo)
        {
            return await _api.ApiKycReportingClientStatByDateFromByDateToGetAsync(dateFrom, dateTo);
        }

        public async Task<IList<KycClientStatRow>> GetKycClientStatsDataShort(DateTime dateFrom, DateTime dateTo)
        {
            return await _api.ApiKycReportingClientStatShortByDateFromByDateToGetAsync(dateFrom, dateTo);
        }

    }
}
