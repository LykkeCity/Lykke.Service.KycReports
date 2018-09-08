using System;
using Autofac;
using Common.Log;
using Lykke.Common.Log;

namespace Lykke.Service.KycReports.Client
{
    public static class AutofacExtension
    {
        /// <summary>
        /// Adds Kyc Reports client to the ContainerBuilder.
        /// </summary>
        /// <param name="builder">ContainerBuilder instance.</param>
        /// <param name="serviceUrl">Effective Kyc Reports service location.</param>
        /// <param name="log">Logger.</param>
        [Obsolete("Please, use the overload without explicitly passed logger.")]
        public static void RegisterKycReportsClient(this ContainerBuilder builder, string serviceUrl, ILog log)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (serviceUrl == null) throw new ArgumentNullException(nameof(serviceUrl));
            if (log == null) throw new ArgumentNullException(nameof(log));
            if (string.IsNullOrWhiteSpace(serviceUrl))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(serviceUrl));

            builder.RegisterInstance(new KycReportsClient(serviceUrl, log)).As<IKycReportsClient>().SingleInstance();
        }

        /// <summary>
        /// Adds Kyc Reports client to the ContainerBuilder.
        /// </summary>
        /// <param name="builder">ContainerBuilder instance. The implementation of ILogFactory should be already injected.</param>
        /// <param name="serviceUrl">Effective Kyc Reports service location.</param>
        public static void RegisterKycReportsClient(this ContainerBuilder builder, string serviceUrl)
        {
            if (string.IsNullOrWhiteSpace(serviceUrl))
                throw new ArgumentException("A reachable Lykke.Service.KycReports URL is required but null or empty string was given.", nameof(serviceUrl));

            builder.Register(ctx => new KycReportsClient(
                    serviceUrl, 
                    ctx.Resolve<ILogFactory>()))
                    .As<IKycReportsClient>()
                    .SingleInstance();
        }
    }
}
