using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Marketplace.Models.Extended
{
    public class ProductImage
    {
        [Required] // example if we need to reduce char count , JsonProperty(PropertyName = "u")]
        public string URL { get; set; }
        public Tag Tag { get; set; }
    }

    public class Tag
    {
        public string VariantID { get; set; }
        public List<string> Options { get; set; }
    }
}
