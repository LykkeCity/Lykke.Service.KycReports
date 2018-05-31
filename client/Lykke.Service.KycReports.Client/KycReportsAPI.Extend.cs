using System;
using System.Net.Http;

// ReSharper disable once CheckNamespace
namespace Lykke.Service.KycReports.AutorestClient
{
    public partial class KycReportsAPI
    {
        /// <inheritdoc />
        /// <summary>
        /// Should be used to prevent memory leak in RetryPolicy
        /// </summary>
        public KycReportsAPI(Uri baseUri, HttpClient client) : base(client)
        {
            Initialize();

            BaseUri = baseUri ?? throw new ArgumentNullException("baseUri");
        }
    }
}
