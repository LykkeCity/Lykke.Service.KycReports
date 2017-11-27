using Autofac;

using AzureStorage.Tables;

using Common.Log;

using Lykke.Service.KycReports.AzureRepositories.Reports;
using Lykke.Service.KycReports.Core.Settings;
using Lykke.SettingsReader;

namespace Lykke.Service.KycReports.AzureRepositories
{
    public static class AzureRepoBinder
    {
        public static void BindAzureRepositories(this ContainerBuilder container, IReloadingManager<DbSettings> dbSettings, ILog log)
        {
            container.RegisterInstance<IKycReportsRepository>(
                new ReportsRepository(
                    AzureTableStorage<ReportRowEntity>.Create(dbSettings.ConnectionString(x => x.ReportsConnString), "Reports", log)));

        }
    }

}
