using System;
using Autofac;
using Common.Log;
using Lykke.Common.Log;

namespace Lykke.Service.KycReports.Client
{
    public static class AutofacExtension
    {
        [Obsolete("Please, use the overload which consumes ILogFactory instead.")]
        public static void RegisterKycReportsClient(this ContainerBuilder builder, string serviceUrl, ILog log)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (serviceUrl == null) throw new ArgumentNullException(nameof(serviceUrl));
            if (log == null) throw new ArgumentNullException(nameof(log));
            if (string.IsNullOrWhiteSpace(serviceUrl))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(serviceUrl));

            builder.RegisterInstance(new KycReportsClient(serviceUrl, log)).As<IKycReportsClient>().SingleInstance();
        }

        public static void RegisterKycReportsClient(this ContainerBuilder builder, string serviceUrl,
            ILogFactory logFactory)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            if (serviceUrl == null)
                throw new ArgumentNullException(nameof(serviceUrl));
            if (logFactory == null)
                throw new ArgumentNullException(nameof(logFactory));
            if (string.IsNullOrWhiteSpace(serviceUrl))
                throw new ArgumentException("A reachable Lykke.Service.KycReports URL is required but null or empty string was given.", nameof(serviceUrl));

            builder.RegisterInstance(
                new KycReportsClient(
                    serviceUrl, 
                    logFactory.CreateLog("KycReportsClient")
                    )
                )
                .As<IKycReportsClient>()
                .SingleInstance();
        }
    }
}
