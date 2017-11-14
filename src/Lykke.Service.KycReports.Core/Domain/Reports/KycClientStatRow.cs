using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.KycReports.Core.Domain.Reports
{
    public class KycClientStatRow
    {
        public DateTime ChangeDate { get; set; }
        public string Date { get; set; }
        public string KycOfficer { get; set; }
        public string KycStatus { get; set; }
        public string PartnerIdName { get; set; } = "";
        public string Id { get; set; } = "";
        public string CountryFromIP { get; set; } = "";
        public string CountryFromID { get; set; } = "";
        public string CountryFromPOA { get; set; } = "";
        public string DateOfExpiryOfID { get; set; } = "";
        public string DateOfPoaDocument { get; set; } = "";
        public string IsDateOfBirthNotEmpty { get; set; }
        public string IsAddressNotEmpty { get; set; }
        public string IsCityNotEmpty { get; set; }
        public string IsZipNotEmpty { get; set; }
        public string IsPhoneInAnotherAccount { get; set; }
        public string IsBanned { get; set; }
        public string kycSpiderCheckDate { get; set; }
        public string isKycSpiderReturnMatches { get; set; }


    }
}
