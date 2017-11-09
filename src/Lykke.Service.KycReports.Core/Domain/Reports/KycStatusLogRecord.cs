using Lykke.Service.Kyc.Abstractions.Domain.Verification;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.KycReports.AzureRepositories.Reports
{
    public class KycStatusLogRecord
    {
        public string ClientId { get; set; }
        public KycStatus StatusCurrent { get; set; }
        public KycStatus StatusPrevious { get; set; }
        public DateTime Date { get; set; }
        public string KycOfficer { get; set; }

        public KycStatusLogRecord(string clientId, KycStatus statusCurrent, KycStatus statusPrevious, DateTime date, string kycOfficer)
        {
            ClientId = clientId;
            StatusCurrent = statusCurrent;
            StatusPrevious = statusPrevious;
            Date = date;
            KycOfficer = kycOfficer;
        }

        public override string ToString()
        {
            return $"{ClientId} {StatusPrevious} => {StatusCurrent} at {Date}";
        }
    }
}
