using Marketplace.Helpers.Attributes;
using Marketplace.Models.Extended;
using OrderCloud.SDK;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace Marketplace.Models
{
    [SwaggerModel]
    public class MarketplaceSpec : Spec<SpecXp>, IMarketplaceObject
    {
       
    }

    [SwaggerModel]
    public class SpecXp
    {
        [Required]
        public SpecUI UI { get; set; } = new SpecUI();
    }

    
}
