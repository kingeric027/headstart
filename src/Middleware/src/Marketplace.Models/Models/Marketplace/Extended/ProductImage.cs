using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ordercloud.integrations.openapispec;

namespace Marketplace.Models.Extended
{
    [SwaggerModel]
    public class ProductImage
    {
        [Required] // example if we need to reduce char count , JsonProperty(PropertyName = "u")]
        public string URL { get; set; }
        public Tag Tag { get; set; }
    }

    [SwaggerModel]
    public class Tag
    {
        public string VariantID { get; set; }
        public List<string> Options { get; set; }
    }
}
