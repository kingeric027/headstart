using Marketplace.Helpers.Attributes;
using Marketplace.Helpers.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.Common.Models
{
    [SwaggerModel]
    public class ImageProductAssignment : ICosmosObject
    {
        [ApiIgnore]
        public string id { get; set; }
        [ApiIgnore]
        public DateTimeOffset timeStamp { get; set; }
        public string ProductID { get; set; }
        public string ImageID { get; set; }
        public List<string> VariantIDs { get; set; }
    }
}
