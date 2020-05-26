using Marketplace.Models.Extended;
using ordercloud.integrations.openapispec;
using OrderCloud.SDK;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace Marketplace.Models
{
    [SwaggerModel]
    public class MarketplaceSpec : Spec, IMarketplaceObject
    {
       
    }

    [SwaggerModel]
    public class SpecXp
    {
        [Required]
        public SpecUI UI { get; set; } = new SpecUI();
    }

    
}
