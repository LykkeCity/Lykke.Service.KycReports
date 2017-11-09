using System;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using Lykke.Service.KycReports.Core.Domain.Reports;

namespace Lykke.Service.KycReports.AzureRepositories.Reports
{
    public class KycOfficerStatsReportEntity : TableEntity
    {
        public static string GeneratePartitionKey(DateTime reportDay)
        {
            return $"REP_{KycReportType.KycOfficerStats}_{reportDay.Ticks}";
        }

        
        public KycReportType ReportType { get; set; }
        public string JsonRow { get; set; }

        public static ReportRowEntity Create(IKycOfficerStatsDataReport rowObj, string rowId)
        {
            var jsonRow = JsonConvert.SerializeObject(rowObj, Formatting.None, new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Unspecified
            });

            return new ReportRowEntity()
            {
                PartitionKey = GeneratePartitionKey(rowObj.ReportDay),
                RowKey = rowId,
                Timestamp = DateTimeOffset.UtcNow,

                ReportType = KycReportType.KycOfficerStats,
                JsonRow = jsonRow
            };
        }

    }
}
