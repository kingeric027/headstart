using Marketplace.Models.Models.Marketplace;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ordercloud.integrations.openapispec;

namespace Marketplace.Models.Misc
{
    [DocIgnore]
    public class EnvironmentSeed
    {
		[Required]
		public List<MarketplaceSupplier> Suppliers { get; set; }
		[Required]
		public List<MarketplaceBuyer> Buyers { get; set; }
		public string ApiUrl { get; set; }
	}
}
