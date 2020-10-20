using Marketplace.Models.Models.Marketplace;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ordercloud.integrations.library;
using SendGrid.Helpers.Mail;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Marketplace.Models.Misc
{
    [DocIgnore]
    public class EnvironmentSeed
    {
		[Required]
		// it is not currently possible to create an organization outside of the devcenter
		// the org will need to be created first in order to be seeded
		public string SellerOrgID { get; set; }
		[Required]
		public List<MarketplaceSupplier> Suppliers { get; set; }
		[Required]
		public List<MarketplaceBuyer> Buyers { get; set; }
	}
}