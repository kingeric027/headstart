using Marketplace.Models.Extended;
using OrderCloud.SDK;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace Marketplace.Models
{
    public class MarketplaceSpec : Spec<SpecXp>, IMarketplaceObject
    {
       
    }

    public class SpecXp
    {
        [Required]
        public SpecUI UI { get; set; } = new SpecUI();
    }

    
}
