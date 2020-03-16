using Marketplace.Models.Models.Marketplace;
using System.Collections.Generic;
using Marketplace.Helpers.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Marketplace.Models.Misc
{
    [DocIgnore]
    public class EnvironmentSeed
    {
		[Required]
		public List<MarketplaceSupplier> Suppliers { get; set; }
		[Required]
		public List<MarketplaceBuyer> Buyers { get; set; }
	}
}
