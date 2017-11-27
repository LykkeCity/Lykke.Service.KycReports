using Lykke.Service.Kyc.Client;
using Lykke.Service.PersonalData.Settings;

namespace Lykke.Service.KycReports.Core.Settings
{
    public class KycReportsSettings
    {
        public DbSettings Db { get; set; }
        public KycServiceSettings KycServiceSettings { get; set; }
        public ServiceSettings Services { get; set; }

    }
}
