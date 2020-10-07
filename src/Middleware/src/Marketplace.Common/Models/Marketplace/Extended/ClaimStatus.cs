using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Marketplace.Models.Extended
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ClaimStatus
    {
        NoClaim,
        Pending,
        Complete,
    }
}
