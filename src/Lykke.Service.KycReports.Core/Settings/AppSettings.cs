using Lykke.Service.ClientAccount.Client;
using Lykke.Service.KycReports.Core.Settings.SlackNotifications;
using Lykke.Service.PersonalData.Settings;

namespace Lykke.Service.KycReports.Core.Settings
{
    public class AppSettings
    {
        public KycReportsSettings KycReportService { get; set; }
        public PersonalDataServiceClientSettings PersonalDataServiceSettings { get; set; }
        public SlackNotificationsSettings SlackNotifications { get; set; }
        public ClientAccountServiceClientSettings ClientAccountServiceClient { get; set; }
    }
}
