using Lykke.Service.KycReports.Core.Domain.Reports;
using System;

namespace Lykke.Service.KycReports.AzureRepositories.Reports
{
    public class KycOfficersPerformanceRow : IKycOfficersPerformanceRow
    {
        public DateTime ReportDay { get; set; }
        public string KycOfficer { get; set; }
        public KycOfficerReportOperationType Operation { get; set; }
        public string ClientEmail { get; set; }
        
        public string ClientId { get; set; }



        public string RowId => $"{ReportDay.Ticks}_{KycOfficerNormalized}_{(int)Operation}_{ClientEmailNormalized}";
        private string KycOfficerNormalized => KycOfficer.ToLower().Trim().Replace(' ', '-').Replace('#', '-');
        private string ClientEmailNormalized => ClientEmail.ToLower().Trim().Replace('@', '-');


        public static string EmptyDayKycOfficer => "### NO_DATA_TODAY ###"; // to determine rows when there were no recods (no need generate data for these days again

        public static KycOfficersPerformanceRow MakeNoDataRow(DateTime reportDate)
        {
            return new KycOfficersPerformanceRow()
            {
                ReportDay = reportDate,
                KycOfficer = KycOfficersPerformanceRow.EmptyDayKycOfficer,
                Operation = KycOfficerReportOperationType.Unknown,
                ClientEmail = string.Empty
            };
        }
    }
}
