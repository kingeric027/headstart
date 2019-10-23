using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Marketplace.Common.Exceptions;

namespace Marketplace.Common.Models
{
    public class WorkItem
    {
        public WorkItem()
        {
        }

        public WorkItem(string path)
        {
            var split = path.Split("/");
            this.SupplierId = split[0];
            this.RecordId = split[2].Replace(".json", "");
            switch (split[1])
            {
                case "product":
                    this.RecordType = RecordType.Product;
                    break;
                case "priceschedule":
                    this.RecordType = RecordType.PriceSchedule;
                    break;
                case "productfacet":
                    this.RecordType = RecordType.ProductFacet;
                    break;
                case "specproductassignment":
                    this.RecordType = RecordType.SpecProductAssignment;
                    break;
                case "specoption":
                    this.RecordType = RecordType.SpecOption;
                    break;
                case "spec":
                    this.RecordType = RecordType.Spec;
                    break;
                default:
                    throw new OrchestrationException(OrchestrationErrorType.WorkItemDefinition, this, path);
            }
        }
        public string SupplierId { get; set; }
        public string RecordId { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public RecordType RecordType { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public Action Action { get; set; }
        public JObject Current { get; set; } // not used for delete
        public JObject Cache { get; set; } // not used for create
        public JObject Diff { get; set; } // only for update
        //[JsonIgnore]
        public OrchestrationSupplier Supplier { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum RecordType
    {
        Product, PriceSchedule, Spec, SpecOption, SpecProductAssignment, ProductFacet
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum Action { Ignore, Create, Update, Patch, Delete, Get }
}
