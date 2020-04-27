using Cosmonaut.Attributes;
using Marketplace.Helpers.Attributes;
using Marketplace.Helpers.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.Common.Models
{
    [SwaggerModel]
    public class Image : ICosmosObject
    {
        [CosmosPartitionKey]
        [JsonProperty(PropertyName = "ID")]
        public string id { get; set; }
        [Sortable]
        [JsonProperty(PropertyName = "DateCreated")]
        public DateTimeOffset timeStamp { get; set; }
        public string URL { get; set; }
        public int ListOrder { get; set; }
        public List<string> Tags = new List<string>();
    }
}
