using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Lykke.Service.KycReports.Core.Domain.Reports
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum KycOfficerReportOperationType
    {
        Unknown,
        OnBoarded,
        Declined,
        ToResubmit
    }
}
