using System;

namespace Lykke.Service.KycReports.AzureRepositories.Reports
{
    public class KycOfficerStatsDataReport : IKycOfficerStatsDataReport
    {
        public DateTime ReportDay { get; set; }
        public string KycOfficer { get; set; }
        public string PartnerName { get; set; }
        public int OnBoardedCount { get; set; }
        public int DeclinedCount { get; set; }
        public int ToResubmitCount { get; set; }

        public string RowId => $"{ReportDay.Ticks}_{KycOfficerNormalized}_{PartnerNameNormalized}";

        private string KycOfficerNormalized => KycOfficer.ToLower().Trim().Replace(' ', '-').Replace('#', '-');
        private string PartnerNameNormalized => PartnerName?.ToLower().Trim().Replace(' ', '-').Replace('#', '-');

        public static string EmptyDayKycOfficer => "### NO_DATA_TODAY ###"; // to determine rows when there were no recods (no need generate data for these days again
        
    }
}
