using Marketplace.Helpers.Attributes;

namespace Marketplace.Models.Misc
{
    [SwaggerModel]
    public class MarketplaceTaxCode
    {
        public string Category { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
    }
}
