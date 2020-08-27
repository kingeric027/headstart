using Cosmonaut.Attributes;
using Newtonsoft.Json;
using ordercloud.integrations.library;
using System.Collections.Generic;
using Newtonsoft.Json.Converters;

namespace Marketplace.Common.Models
{
    [SwaggerModel]
    [CosmosCollection("reporttemplates")]
    public class ReportTemplate : CosmosObject
    {
        [JsonProperty("id")]
        public string ID { get; set; }
        [CosmosPartitionKey]
        public string SellerID { get; set; }
        public ReportTypeEnum ReportType { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> Headers { get; set; }
        public ReportFilters Filters { get; set; }
        public bool AvailableToSuppliers { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ReportTypeEnum
    {
        BuyerLocation
    }

    public class ReportFilters
    {
        public List<string> BuyerID { get; set; }
        public List<string> Country { get; set; }
        public List<string> State { get; set; }
    }
}
