using Marketplace.Common.Exceptions;
using Marketplace.Models.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace Marketplace.Models.Misc
{
    public class WorkItem
    {
        public WorkItem()
        {
        }

        public WorkItem(string path)
        {
            var split = path.Split("/");
            this.ResourceId = split[0];
            this.RecordId = split[split.Length - 1].Replace(".json", "");
            switch (split[2])
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
                case "buyer":
                    this.RecordType = RecordType.Buyer;
                    break;
                case "user":
                    this.RecordType = RecordType.User;
                    break;
                case "usergroup":
                    this.RecordType = RecordType.UserGroup;
                    break;
                case "address":
                    this.RecordType = RecordType.Address;
                    break;
                case "usergroupassignment":
                    this.RecordType = RecordType.UserGroupAssignment;
                    break;
                case "addressassignment":
                    this.RecordType = RecordType.AddressAssignment;
                    break;
                case "costcenter":
                    this.RecordType = RecordType.CostCenter;
                    break;
                case "catalogassignment":
                    this.RecordType = RecordType.CatalogAssignment;
                    break;
                case "catalog":
                    this.RecordType = RecordType.Catalog;
                    break;
                default:
                    throw new OrchestrationException(OrchestrationErrorType.WorkItemDefinition, path);
            }
        }
        public string ResourceId { get; set; }
        public string RecordId { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public RecordType RecordType { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public Action Action { get; set; }
        public JObject Current { get; set; } // not used for delete
        public JObject Cache { get; set; } // not used for create
        public JObject Diff { get; set; } // only for update
        public string Token { get; set; }
        public string ClientId { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum RecordType
    {
        Product, PriceSchedule, Spec, SpecOption, SpecProductAssignment, ProductFacet,
        Buyer, User, UserGroup, Address, CostCenter, UserGroupAssignment, AddressAssignment, 
        CatalogAssignment, Catalog
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum Action { Ignore, Create, Update, Patch, Delete, Get }
}
