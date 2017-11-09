using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common.Log;
using Lykke.Service.Kyc.Abstractions.Services;
using Lykke.Service.Kyc.Client;
using Lykke.Service.KycReports.AzureRepositories;
using Lykke.Service.KycReports.Core.Domain.Reports;
using Lykke.Service.KycReports.Core.Services;
using Lykke.Service.KycReports.Core.Settings.ServiceSettings;
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
        private readonly IReloadingManager<PersonalDataServiceSettings> _personalDataServiceSettings;
        private readonly ILog _log;
        // NOTE: you can remove it if you don't need to use IServiceCollection extensions to register service specific dependencies
        private readonly IServiceCollection _services;

        public ServiceModule(IReloadingManager<KycReportsSettings> settings, IReloadingManager<PersonalDataServiceSettings> personalDataServiceSettings, ILog log)
        {
            _settings = settings;
            _personalDataServiceSettings = personalDataServiceSettings;
            _log = log;

            _services = new ServiceCollection();
        }

        protected override void Load(ContainerBuilder builder)
        {
            // TODO: Do not register entire settings in container, pass necessary settings to services which requires them
            // ex:
            //  builder.RegisterType<QuotesPublisher>()
            //      .As<IQuotesPublisher>()
            //      .WithParameter(TypedParameter.From(_settings.CurrentValue.QuotesPublication))

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
            builder.RegisterInstance<KycServiceSettings>(_settings.CurrentValue.KycServiceSettings).SingleInstance(); // kyc service 


            builder.Populate(_services);
        }
    }
}
