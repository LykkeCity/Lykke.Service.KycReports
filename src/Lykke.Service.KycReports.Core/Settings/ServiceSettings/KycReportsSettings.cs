using Lykke.Service.Kyc.Client;
using Lykke.Service.PersonalData.Settings;

namespace Lykke.Service.KycReports.Core.Settings.ServiceSettings
{
    public class KycReportsSettings
    {
        public DbSettings Db { get; set; }
        public KycServiceSettings KycServiceSettings { get; set; }
    }
}
