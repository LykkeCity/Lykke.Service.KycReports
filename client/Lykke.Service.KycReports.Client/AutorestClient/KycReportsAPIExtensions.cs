// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Lykke.Service.KycReports.AutorestClient
{
    using Models;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Extension methods for KycReportsAPI.
    /// </summary>
    public static partial class KycReportsAPIExtensions
    {
            /// <summary>
            /// Checks service is alive
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            public static object IsAlive(this IKycReportsAPI operations)
            {
                return operations.IsAliveAsync().GetAwaiter().GetResult();
            }

            /// <summary>
            /// Checks service is alive
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<object> IsAliveAsync(this IKycReportsAPI operations, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.IsAliveWithHttpMessagesAsync(null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='dateFrom'>
            /// </param>
            /// <param name='dateTo'>
            /// </param>
            public static string ApiKycReportingStatsByDateFromByDateToGet(this IKycReportsAPI operations, System.DateTime dateFrom, System.DateTime dateTo)
            {
                return operations.ApiKycReportingStatsByDateFromByDateToGetAsync(dateFrom, dateTo).GetAwaiter().GetResult();
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='dateFrom'>
            /// </param>
            /// <param name='dateTo'>
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<string> ApiKycReportingStatsByDateFromByDateToGetAsync(this IKycReportsAPI operations, System.DateTime dateFrom, System.DateTime dateTo, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.ApiKycReportingStatsByDateFromByDateToGetWithHttpMessagesAsync(dateFrom, dateTo, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='dateFrom'>
            /// </param>
            /// <param name='dateTo'>
            /// </param>
            public static string ApiKycReportingPerformByDateFromByDateToGet(this IKycReportsAPI operations, System.DateTime dateFrom, System.DateTime dateTo)
            {
                return operations.ApiKycReportingPerformByDateFromByDateToGetAsync(dateFrom, dateTo).GetAwaiter().GetResult();
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='dateFrom'>
            /// </param>
            /// <param name='dateTo'>
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<string> ApiKycReportingPerformByDateFromByDateToGetAsync(this IKycReportsAPI operations, System.DateTime dateFrom, System.DateTime dateTo, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.ApiKycReportingPerformByDateFromByDateToGetWithHttpMessagesAsync(dateFrom, dateTo, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='dateFrom'>
            /// </param>
            /// <param name='dateTo'>
            /// </param>
            public static string ApiKycReportingLeadershipByDateFromByDateToGet(this IKycReportsAPI operations, System.DateTime dateFrom, System.DateTime dateTo)
            {
                return operations.ApiKycReportingLeadershipByDateFromByDateToGetAsync(dateFrom, dateTo).GetAwaiter().GetResult();
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='dateFrom'>
            /// </param>
            /// <param name='dateTo'>
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<string> ApiKycReportingLeadershipByDateFromByDateToGetAsync(this IKycReportsAPI operations, System.DateTime dateFrom, System.DateTime dateTo, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.ApiKycReportingLeadershipByDateFromByDateToGetWithHttpMessagesAsync(dateFrom, dateTo, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            public static bool? ApiKycReportingRebuildStatsPost(this IKycReportsAPI operations)
            {
                return operations.ApiKycReportingRebuildStatsPostAsync().GetAwaiter().GetResult();
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<bool?> ApiKycReportingRebuildStatsPostAsync(this IKycReportsAPI operations, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.ApiKycReportingRebuildStatsPostWithHttpMessagesAsync(null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            public static bool? ApiKycReportingRebuildPerformPost(this IKycReportsAPI operations)
            {
                return operations.ApiKycReportingRebuildPerformPostAsync().GetAwaiter().GetResult();
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<bool?> ApiKycReportingRebuildPerformPostAsync(this IKycReportsAPI operations, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.ApiKycReportingRebuildPerformPostWithHttpMessagesAsync(null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='dateFrom'>
            /// </param>
            /// <param name='dateTo'>
            /// </param>
            public static IList<KycClientStatRow> ApiKycReportingClientStatByDateFromByDateToGet(this IKycReportsAPI operations, System.DateTime dateFrom, System.DateTime dateTo)
            {
                return operations.ApiKycReportingClientStatByDateFromByDateToGetAsync(dateFrom, dateTo).GetAwaiter().GetResult();
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='dateFrom'>
            /// </param>
            /// <param name='dateTo'>
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<IList<KycClientStatRow>> ApiKycReportingClientStatByDateFromByDateToGetAsync(this IKycReportsAPI operations, System.DateTime dateFrom, System.DateTime dateTo, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.ApiKycReportingClientStatByDateFromByDateToGetWithHttpMessagesAsync(dateFrom, dateTo, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='dateFrom'>
            /// </param>
            /// <param name='dateTo'>
            /// </param>
            public static IList<KycClientStatRow> ApiKycReportingClientStatShortByDateFromByDateToGet(this IKycReportsAPI operations, System.DateTime dateFrom, System.DateTime dateTo)
            {
                return operations.ApiKycReportingClientStatShortByDateFromByDateToGetAsync(dateFrom, dateTo).GetAwaiter().GetResult();
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='dateFrom'>
            /// </param>
            /// <param name='dateTo'>
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<IList<KycClientStatRow>> ApiKycReportingClientStatShortByDateFromByDateToGetAsync(this IKycReportsAPI operations, System.DateTime dateFrom, System.DateTime dateTo, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.ApiKycReportingClientStatShortByDateFromByDateToGetWithHttpMessagesAsync(dateFrom, dateTo, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

    }
}
