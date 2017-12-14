using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Lykke.Service.KycReports.Core.Domain.Reports;
using System.Collections.Generic;
using Lykke.Service.Kyc.Abstractions.Domain.Verification;

namespace Lykke.Service.KycReports.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    public class KycReportingController : Controller
    {
        private readonly IKycReportingService _kycReportingService;

        public KycReportingController(IKycReportingService kycReportingService) {
            _kycReportingService = kycReportingService;
        }

        [HttpGet]
        [Route("stats/{dateFrom}/{dateTo}")]
        public async Task<string> GetKycOfficerStatsJsonAsync(DateTime dateFrom, DateTime dateTo)
        {
            return await _kycReportingService.GetKycOfficerStatsJsonAsync(dateFrom.Date, dateTo.Date);
        }

        [HttpGet]
        [Route("perform/{dateFrom}/{dateTo}")]
        public async Task<string> GetKycOfficersPerformanceJsonAsync(DateTime dateFrom, DateTime dateTo)
        {
            return await _kycReportingService.GetKycOfficersPerformanceJsonAsync(dateFrom.Date, dateTo.Date);
        }

        [HttpGet]
        [Route("leadership/{dateFrom}/{dateTo}")]
        public async Task<string> GetKycReportDailyLeadershipDataJsonAsync(DateTime dateFrom, DateTime dateTo)
        {
            return await _kycReportingService.GetKycReportDailyLeadershipDataJsonAsync(dateFrom.Date, dateTo.Date);
        }

        [HttpPost]
        [Route("rebuild-stats")]
        public async Task<bool> RebuildKycOfficerStats()
        {
            return await _kycReportingService.RebuildKycOfficerStats();
        }

        [HttpPost]
        [Route("rebuild-perform")]
        public async Task<bool> RebuildKycOfficersPerformance()
        {
            return await _kycReportingService.RebuildKycOfficersPerformance();
        }

        [HttpGet]
        [Route("clientStat/{dateFrom}/{dateTo}")]
        public async Task<IEnumerable<KycClientStatRow>> GetKycClientStatsData(DateTime dateFrom, DateTime dateTo)
        {
            var rows = await _kycReportingService.GetKycClientStatRows(dateFrom.Date, dateTo.Date);
            return rows;
        }

        [HttpGet]
        [Route("clientStatShort/{dateFrom}/{dateTo}")]
        public async Task<IEnumerable<KycClientStatRow>> GetKycClientStatsDataShort(DateTime dateFrom, DateTime dateTo)
        {
            var rows = await _kycReportingService.GetKycClientStatRows(dateFrom.Date, dateTo.Date, new KycStatus[] { KycStatus.Ok, KycStatus.ReviewDone });
            return rows;
        }

    }
}
