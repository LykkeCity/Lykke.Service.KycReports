using System;

using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using Lykke.Service.KycReports.Core.Domain.Reports;

namespace Lykke.Service.KycReports.AzureRepositories.Reports
{
    public class ReportRowEntity : TableEntity
    {
        public static string GeneratePartitionKey(KycReportType reportType)
        {
            return $"REP_{reportType}";
        }

        public KycReportType ReportType { get; set; }
        public string JsonRow { get; set; }
        
        public static ReportRowEntity Create<T>(KycReportType reportType, T rowObj, string rowId)
        {
            var jsonRow = JsonConvert.SerializeObject(rowObj, Formatting.None, new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Unspecified
            });

            return new ReportRowEntity()
            {
                PartitionKey = GeneratePartitionKey(reportType),
                RowKey = rowId,
                Timestamp = DateTimeOffset.UtcNow,
                
                ReportType = reportType,
                JsonRow = jsonRow
            };
        }
    }
}
