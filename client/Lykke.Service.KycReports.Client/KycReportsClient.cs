using System;
using Common.Log;
using Lykke.Service.KycReports.AutorestClient;
using System.Threading.Tasks;

namespace Lykke.Service.KycReports.Client
{
    public class KycReportsClient : IKycReportsClient, IDisposable
    {
        private readonly ILog _log;
        private readonly IKycReportsAPI _api;

        public KycReportsClient(string serviceUrl, ILog log)
        {

            _log = log;
            _api = new KycReportsAPI(new Uri(serviceUrl));
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


        public void Dispose()
        {
            //if (_service == null)
            //    return;
            //_service.Dispose();
            //_service = null;
        }
    }
}
