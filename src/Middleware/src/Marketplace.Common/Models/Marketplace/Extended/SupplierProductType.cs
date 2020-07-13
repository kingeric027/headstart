using System.Text.Json.Serialization;

namespace Marketplace.Common.Models.Marketplace.Extended
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum SupplierProductType
    {
        Standard,
        Quote,
        PurchaseOrder
    }
}
