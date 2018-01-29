using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common.Log;
using Lykke.Service.ClientAccount.Client;
using Lykke.Service.Kyc.Abstractions.Services;
using Lykke.Service.Kyc.Client;
using Lykke.Service.KycReports.AzureRepositories;
using Lykke.Service.KycReports.Core.Domain.Reports;
using Lykke.Service.KycReports.Core.Services;
using Lykke.Service.KycReports.Core.Settings;
using Lykke.Service.KycReports.Core.Settings.ClientAccount;
using Lykke.Service.KycReports.Services;
using Lykke.Service.KycReports.Services.Reports;
using Lykke.Service.PersonalData.Client;
using Lykke.Service.PersonalData.Contract;
using Lykke.Service.PersonalData.Settings;
using Lykke.SettingsReader;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Service.KycReports.Modules
{
    public class ServiceModule : Module
    {
        private readonly IReloadingManager<KycReportsSettings> _settings;
        private readonly IReloadingManager<PersonalDataServiceClientSettings> _personalDataServiceSettings;
        private readonly IReloadingManager<ClientAccountServiceClientSettings> _clientAccountServiceSettings;
        private readonly ILog _log;

        public ServiceModule(IReloadingManager<KycReportsSettings> settings, IReloadingManager<PersonalDataServiceClientSettings> personalDataServiceSettings, IReloadingManager<ClientAccountServiceClientSettings> clientAccountServiceSettings, ILog log)
        {
            _settings = settings;
            _personalDataServiceSettings = personalDataServiceSettings;
            _clientAccountServiceSettings = clientAccountServiceSettings;
            _log = log;
        }

        protected override void Load(ContainerBuilder builder)
        {

            builder.RegisterInstance(_log)
                .As<ILog>()
                .SingleInstance();

            builder.RegisterType<HealthService>()
                .As<IHealthService>()
                .SingleInstance();

            builder.RegisterType<StartupManager>()
                .As<IStartupManager>();

            builder.RegisterType<ShutdownManager>()
                .As<IShutdownManager>();

            // TODO: Add your dependencies here

            builder.BindAzureRepositories(_settings.Nested(x => x.Db), _log);


            builder.RegisterInstance<IPersonalDataService>(new PersonalDataService(_personalDataServiceSettings.CurrentValue, _log));
            builder.RegisterType<KycReportingService>().As<IKycReportingService>().SingleInstance();

            builder.RegisterType<KycStatusServiceClient>().As<IKycStatusService>().SingleInstance(); // kyc service 
            builder.RegisterInstance(_settings.CurrentValue.KycServiceSettings).SingleInstance(); // kyc service 

            builder.RegisterLykkeServiceClient(_clientAccountServiceSettings.CurrentValue.ServiceUrl);
        }
    }
}
