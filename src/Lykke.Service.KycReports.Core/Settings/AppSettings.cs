using Lykke.Service.KycReports.Core.Settings.ServiceSettings;
using Lykke.Service.KycReports.Core.Settings.SlackNotifications;
using Lykke.Service.PersonalData.Settings;

namespace Lykke.Service.KycReports.Core.Settings
{
    public class AppSettings
    {
        public KycReportsSettings KycReportService { get; set; }
        public PersonalDataServiceSettings PersonalDataServiceSettings { get; set; }
        public SlackNotificationsSettings SlackNotifications { get; set; }
    }
}
