using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AzureStorage;
using Lykke.Service.KycReports.Core.Domain.Reports;

using Newtonsoft.Json;

namespace Lykke.Service.KycReports.AzureRepositories.Reports
{
    public class ReportsRepository : IKycReportsRepository
    {
        private readonly INoSQLTableStorage<ReportRowEntity> _tableStorage;

        public ReportsRepository(INoSQLTableStorage<ReportRowEntity> tableStorage)
        {
            _tableStorage = tableStorage;
        } 

        public async Task InsertRow(IKycReportDailyLeadership row)
        {
            var reportDataRow = ReportRowEntity.Create(KycReportType.KycReportDailyLeadership, row, row.ReportDay.Ticks.ToString());
            await _tableStorage.InsertOrReplaceAsync(reportDataRow);
        }

        public async Task InsertRow(IKycOfficerStatsDataReport row)
        {
            var reportDataRow = KycOfficerStatsReportEntity.Create(row, row.RowId);
            await _tableStorage.InsertOrReplaceAsync(reportDataRow);
        }

        public async Task InsertRow(IKycOfficersPerformanceRow row)
        {
            var reportDataRow = KycOfficersPerformanceReportEntity.Create(row, row.RowId);
            await _tableStorage.InsertOrReplaceAsync(reportDataRow);
        }

        public async Task<IEnumerable<string>> GetJsonData(KycReportType reportType, bool isDescending = false, IEnumerable<string> rowKeys = null)
        {
            string partitionKey;

            switch (reportType)
            {
                case KycReportType.KycReportDailyLeadership:
                    partitionKey = ReportRowEntity.GeneratePartitionKey(KycReportType.KycReportDailyLeadership);
                    break;
                default:
                    partitionKey = null;
                    break;
            }


            if (!string.IsNullOrWhiteSpace(partitionKey))
            {
                IEnumerable<ReportRowEntity> data;
                if (rowKeys == null)
                    data = await _tableStorage.GetDataAsync(partitionKey);
                else
                    data = await _tableStorage.GetDataAsync(partitionKey, rowKeys);

                var jsonRows = isDescending
                    ? data.OrderByDescending(d => d.Timestamp).Select(d => d.JsonRow)
                    : data.Select(d => d.JsonRow);
                
                //var jsonReport = $"[\r\n{string.Join(", \r\n", jsonRows)}\r\n]";

                return jsonRows;
            }

            return new List<string>();
        }


        private async Task<IEnumerable<T>> GetReportRows<T>(string partitionKey, IEnumerable<string> rowKeys = null)
        {
            IEnumerable<ReportRowEntity> data;
            if (rowKeys == null)
                data = await _tableStorage.GetDataAsync(partitionKey);
            else
                data = await _tableStorage.GetDataAsync(partitionKey, rowKeys);

            return data.Select(d => JsonConvert.DeserializeObject<T>(d.JsonRow));
        }

        public async Task<List<string>> GetKycOfficerStatsJsonRows(DateTime from, DateTime to)
        {
            var startDate = new DateTime(from.Year, from.Month, from.Day);
            var endDate = new DateTime(to.Year, to.Month, to.Day);

            var datesArray = Enumerable.Range(0, 1 + endDate.Subtract(startDate).Days)
                  .Select(offset => startDate.AddDays(offset))
                  .ToArray();

            var jsonRows = new List<ReportRowEntity>();

            foreach (var reportDay in datesArray)
            {
                var partitionKey = KycOfficerStatsReportEntity.GeneratePartitionKey(reportDay);
                var data = await _tableStorage.GetDataAsync(partitionKey);
                jsonRows.AddRange(data);
            }
            
            return jsonRows
                .OrderByDescending(d => d.Timestamp)
                .Select(d => d.JsonRow)
                .ToList();
        }

        public async Task<List<string>> GetKycOfficersPerformanceJsonRows(DateTime from, DateTime to)
        {
            var startDate = new DateTime(from.Year, from.Month, from.Day);
            var endDate = new DateTime(to.Year, to.Month, to.Day);

            var datesArray = Enumerable.Range(0, 1 + endDate.Subtract(startDate).Days)
                  .Select(offset => startDate.AddDays(offset))
                  .ToArray();

            var jsonRows = new List<ReportRowEntity>();

            foreach (var reportDay in datesArray)
            {
                var partitionKey = KycOfficersPerformanceReportEntity.GeneratePartitionKey(reportDay);
                var data = await _tableStorage.GetDataAsync(partitionKey);
                jsonRows.AddRange(data);
            }

            return jsonRows
                .Select(d => d.JsonRow)
                .ToList();
        }
    }
}
