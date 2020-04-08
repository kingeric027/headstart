using Marketplace.Helpers.Attributes;
using Marketplace.Helpers.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.Common.Models
{
    [SwaggerModel]
    public class Image : ICosmosObject
    {
        public string id { get; set; }
        [Sortable]
        public DateTimeOffset timeStamp { get; set; }
        public string URL { get; set; }
        public List<string> VariantIDs { get; set; }
    }
}
